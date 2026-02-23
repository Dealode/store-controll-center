namespace BusinessControl.Shared.Procurement;

public sealed class VendorDto
{
    public Guid Id { set; get; }
    public string Name { set; get; }
    public BuyMarketplaceDto BuyMarketplace { set; get; }
    public string? Url { set; get; }
}