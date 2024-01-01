using EventPublisher;
using Grpc.Core;
using GrpcOrderService.Dto;
using Newtonsoft.Json;

namespace GrpcOrderService.Services
{
    public class OrderService : Order.OrderBase
    {
        private readonly ILogger<OrderService> _logger;
        private readonly string JsonResourcePath = "Data/orders.json";
        private readonly object LockObject = new object();
        private readonly List<OrderDto> Orders = new List<OrderDto>();
        private readonly RabbitMQMessagePublisher _orderCreationMessagePublisher;
        private readonly RabbitMQMessagePublisher _orderUpdationMessagePublisher;

        public OrderService(ILogger<OrderService> logger, RabbitMQSetting rabbitMQSetting)
        {
            _logger = logger;
            _orderCreationMessagePublisher = new RabbitMQMessagePublisher(rabbitMQSetting, "order_create_exchange", "fanout", "order_create_queue");
            _orderUpdationMessagePublisher = new RabbitMQMessagePublisher(rabbitMQSetting, "order_update_exchange", "topic", "order_update_queue");
            LoadOrdersFromJsonFile();
        }

        public override Task<OrderResponse> PlaceOrder(CreateOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received: place order request");

            try
            {
                lock (LockObject)
                {
                    if (request.ProductDetail == null)
                    {
                        return Task.FromResult(new OrderResponse
                        {
                            Success = false,
                            Message = "Product detail is null."
                        });
                    }

                    // Place an order
                    var newOrder = new OrderDto
                    {
                        OrderId = Random.Shared.Next(),
                        ProductId = request.ProductDetail.ProductId,
                        Quantity = request.Quantity,
                        UnitPrice = request.ProductDetail.Price,
                        TotalPrice = request.Quantity * request.ProductDetail.Price
                    };

                    Orders.Add(newOrder);

                    // Save orders to the JSON file
                    SaveOrdersToJsonFile();

                    var message = $"Order placed successfully for order id: {newOrder.OrderId}.";

                    //publish to notification 1 & 2.
                    _orderCreationMessagePublisher.PublishMessage(message, "creation");
                    context.Status = new Status(StatusCode.OK, message);
                    return Task.FromResult(new OrderResponse
                    {
                        Success = true,
                        Message = message,
                        OrderId = newOrder.OrderId
                    });
                }
            }
            catch (Exception ex)
            {
                var message = $"Error: internal server error while placing order for product id: {request?.ProductDetail?.ProductId}";
                _logger.LogError(message, ex);
                context.Status = new Status(StatusCode.Internal, message);
                return Task.FromResult(new OrderResponse { Success = false, Message = message });
            }
            finally
            {
                _logger.LogInformation("Place order request sucessfully completed!!");
            }
        }

        public override Task<OrderResponse> UpdateOrder(UpdateOrderRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received: update order request");

            try
            {
                lock (LockObject)
                {
                    if (request.ProductId == 0)
                    {
                        return Task.FromResult(new OrderResponse
                        {
                            Success = false,
                            Message = "Product detail is invalid."
                        });
                    }

                    if (request.OrderId == 0)
                    {
                        return Task.FromResult(new OrderResponse
                        {
                            Success = false,
                            Message = "Order id is invalid."
                        });
                    }


                    var existingOrder = Orders.FirstOrDefault(o => o.OrderId == request.OrderId && o.ProductId == request.ProductId);
                    if (existingOrder != null)
                    {
                        // Update order details
                        existingOrder.Quantity = request.Quantity;
                        existingOrder.TotalPrice = request.Quantity * existingOrder.UnitPrice;

                        SaveOrdersToJsonFile();

                        var message = $"Order updated successfully for order id: {request.OrderId}.";

                        //publish to notification 2.
                        _orderUpdationMessagePublisher.PublishMessage(message, "updation");
                        context.Status = new Status(StatusCode.OK, message);
                        return Task.FromResult(new OrderResponse { 
                            Success = true, 
                            Message = message,
                            OrderId = request.OrderId
                        });
                    }
                    else
                    {
                        return Task.FromResult(new OrderResponse { 
                            Success = false, 
                            Message = "Order not found." 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var message = $"Error: internal server error while updating order for order id: {request.OrderId}";
                _logger.LogError(message, ex);
                context.Status = new Status(StatusCode.Internal, message);
                return Task.FromResult(new OrderResponse { Success = false, Message = message });
            }
            finally
            {
                _logger.LogInformation("Update order request sucessfully completed!!");
            }
        }

        private void SaveOrdersToJsonFile()
        {
            var json = JsonConvert.SerializeObject(Orders, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(JsonResourcePath, json);
        }

        private void LoadOrdersFromJsonFile()
        {
            if (File.Exists(JsonResourcePath))
            {
                var json = File.ReadAllText(JsonResourcePath);
                var jsonData = JsonConvert.DeserializeObject<List<OrderDto>>(json);
                if (jsonData != null)
                {
                    Orders.AddRange(jsonData);
                }
            }
        }
    }
}
