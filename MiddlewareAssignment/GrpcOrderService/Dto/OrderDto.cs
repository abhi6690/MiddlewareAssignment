namespace GrpcOrderService.Dto
{
    public class OrderDto
    {
        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public float UnitPrice { get; set; }

        public float TotalPrice { get; set; }
    }
}
