using BusinessControl.Shared.Procurement;

namespace BusinessControl.Domain.Procurement.Calculator;

public static class LandedCostCalculator
{
    public static LandedCostResult Calculate(VendorOfferDto offerDto, LandedCostRequest req)
    {
        if (req.Qty <= 0) throw new ArgumentOutOfRangeException(nameof(req.Qty));
        if (req.ExchangeRateToTarget <= 0) throw new ArgumentOutOfRangeException(nameof(req.ExchangeRateToTarget));

        decimal priceForQty = offerDto.PriceForQty(req.Qty);

        return new LandedCostResult
        {
            OfferId = offerDto.Id,
            Qty = req.Qty,
            OfferCurrency = offerDto.Currency,
            TargetCurrency = req.TargetCurrency,

            Moq = offerDto.Moq,
            MoqSatisfied = req.Qty >= offerDto.Moq,

            Price = priceForQty * req.Qty * req.ExchangeRateToTarget,
        };
    }
}