namespace ProductService.Dto;

public class OrderResponseDto
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public Guid OrderId { get; set; }
}