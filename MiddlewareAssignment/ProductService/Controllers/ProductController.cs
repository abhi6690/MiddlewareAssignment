using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Dto;

namespace ProductService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly Order.OrderClient _orderClient;
    private static List<ProductDetailDto> Products = new List<ProductDetailDto>();

    public ProductController(IConfiguration configuration)
    {
        // from launchSettings.json in GrpcOrderService.
        var url = configuration["GrpcOrderService.ApplicationUrl"] ?? "https://localhost:7211";
        var channel = GrpcChannel.ForAddress(url);
        _orderClient = new Order.OrderClient(channel);
        LoadProducts();
    }

    [HttpGet("all-products")]
    public ActionResult<IEnumerable<ProductDetailDto>> GetAllProducts()
    {
        return Products;
    }

    [HttpGet("{productId}")]
    public ActionResult<ProductDetailDto> GetProduct(int productId)
    {
        var product = Products.FirstOrDefault(x => x.ProductId == productId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return product;
    }

    [HttpPost("order/place-order")]
    public async Task<ActionResult> PlaceOrder([FromBody] CreateOrderRequestDto request)
    {
        var product = Products.FirstOrDefault(x => request.ProductId == x.ProductId);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        var createOrderRequest = new CreateOrderRequest
        {
            Quantity = request.Quantity,
            ProductDetail = new ProductDetail
            {
                ProductId = product.ProductId,
                Color = product.Color,
                Description = product.Description,
                Price = product.Price
            }
        };

        var result = await _orderClient.PlaceOrderAsync(createOrderRequest);
        return result.OrderId != 0 ? Ok(result.OrderId) : BadRequest(result.Message);
    }

    [HttpPut("order/update-order")]
    public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderRequestDto request)
    {
        var product = Products.FirstOrDefault(x => request.ProductId == x.ProductId);
        if (product == null)
        {
            return NotFound("Product not found");
        }

        var updateOrderRequest = new UpdateOrderRequest
        {
            OrderId = request.OrderId,
            Quantity = request.Quantity,
            ProductId = request.ProductId
        };
        var result = await _orderClient.UpdateOrderAsync(updateOrderRequest);
        return result.OrderId != 0 ? Ok(result.OrderId) : BadRequest(result.Message);
    }

    private void LoadProducts()
    {
      Products = new List<ProductDetailDto>
      {
          new ProductDetailDto { ProductId = 1234, Color = "Red", Description = "High-quality", Price = 45.99F },
          new ProductDetailDto { ProductId = 5678, Color = "Blue", Description = "Modern", Price = 29.99F },
          new ProductDetailDto { ProductId = 7890, Color = "Green", Description = "Durable", Price = 89.99F },
          new ProductDetailDto { ProductId = 4321, Color = "Yellow", Description = "Classic", Price = 19.99F },
          new ProductDetailDto { ProductId = 8765, Color = "Black", Description = "Fashionable", Price = 59.99F },
          new ProductDetailDto { ProductId = 2345, Color = "White", Description = "High-quality", Price = 39.99F },
          new ProductDetailDto { ProductId = 6789, Color = "Red", Description = "Durable", Price = 79.99F },
          new ProductDetailDto { ProductId = 8901, Color = "Blue", Description = "Modern", Price = 49.99F },
          new ProductDetailDto { ProductId = 3456, Color = "Green", Description = "Classic", Price = 69.99F },
          new ProductDetailDto { ProductId = 9012, Color = "Yellow", Description = "Fashionable", Price = 34.99F }
      };
    }
}