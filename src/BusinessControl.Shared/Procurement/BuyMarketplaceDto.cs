namespace BusinessControl.Shared.Procurement;

public sealed class BuyMarketplaceDto
{
    public Guid Id { set; get; }
    public string Name { set; get; }
    public string Country { set; get; }
    public decimal PaymentFeePercent { set; get; }
}