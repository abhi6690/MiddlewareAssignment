namespace ProductService.Dto
{
    public class UpdateOrderRequestDto
    {
        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public int ProductId { get; set; }
    }
}
