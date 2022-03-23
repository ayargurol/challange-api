namespace Challenge.Application.Dto.Product.Request;

public class ProductUpdateRequest
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }

}