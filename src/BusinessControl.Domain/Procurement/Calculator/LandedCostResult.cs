using BusinessControl.Domain.Procurement.Enums;

namespace BusinessControl.Domain.Procurement.Calculator;

public sealed class LandedCostResult
{
    public Guid OfferId { get; init; }
    public int Qty { get; init; }
    
    public CurrencyCode OfferCurrency { get; init; }
    public CurrencyCode TargetCurrency { get; init; }
    
    public int Moq { get; init; }
    public bool MoqSatisfied { get; init; }
    
    public decimal Price { get; init; }
    
}