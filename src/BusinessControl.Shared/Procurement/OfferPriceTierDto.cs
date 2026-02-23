namespace BusinessControl.Shared.Procurement;

public sealed class OfferPriceTierDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int MinQty { get; set; }
    public int? MaxQty { get; set; }
    public decimal Price { get; set; }
}