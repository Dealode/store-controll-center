namespace BusinessControl.Shared.Procurement;

public sealed class ProductDto
{
    public Guid Id { set; get; }
    public string Sku { set; get; }
    public string Name { set; get; }
    public float Length { set; get; }
    public float Width { set; get; }
    public float Height { set; get; }
    public float Weight { set; get; }
}