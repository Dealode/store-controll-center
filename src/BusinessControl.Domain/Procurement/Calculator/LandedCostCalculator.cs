namespace BusinessControl.Domain.Procurement.Calculator;

public static class LandedCostCalculator
{
    public static LandedCostResult Calculate(VendorOffer offer, LandedCostRequest req)
    {
        if (req.Qty <= 0) throw new ArgumentOutOfRangeException(nameof(req.Qty));
        if (req.ExchangeRateToTarget <= 0) throw new ArgumentOutOfRangeException(nameof(req.ExchangeRateToTarget));

        decimal priceForQty = offer.PriceForQty(req.Qty);

        return new LandedCostResult
        {
            OfferId = offer.Id,
            Qty = req.Qty,
            OfferCurrency = offer.Currency,
            TargetCurrency = req.TargetCurrency,

            Moq = offer.Moq,
            MoqSatisfied = req.Qty >= offer.Moq,

            Price = priceForQty * req.Qty * req.ExchangeRateToTarget,
        };
    }
}