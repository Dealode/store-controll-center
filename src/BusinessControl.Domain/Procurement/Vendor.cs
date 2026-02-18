namespace BusinessControl.Domain.Procurement;

public sealed class Vendor
{
    public Guid Id { set; get; }
    public string Name { set; get; }
    public BuyMarketplace BuyMarketplace { set; get; }
    public string? Url { set; get; }
}