namespace BusinessControl.Domain.Procurement;

public sealed class BuyMarketplace
{
    public Guid Id { set; get; }
    public string Name { set; get; }
    public string Country { set; get; }
    public decimal PaymentFeePercent { set; get; }
}