using BusinessControl.Domain.Procurement.Enums;

namespace BusinessControl.Domain.Procurement.Calculator;

public sealed class LandedCostRequest
{
    public Guid OfferId { get; init; }
    public int Qty { get; init; }

    public CurrencyCode TargetCurrency { get; init; } = CurrencyCode.Uah;
    public decimal ExchangeRateToTarget { get; init; } = 1m;
}