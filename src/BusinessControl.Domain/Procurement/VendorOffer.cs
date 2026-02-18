using BusinessControl.Domain.Procurement.Enums;

namespace BusinessControl.Domain.Procurement;

public sealed class VendorOffer
{
    public Guid Id { set; get; }
    
    public Guid ProductId { set; get; }
    public Guid VendorId { set; get; }
    
    public CurrencyCode Currency { set; get; }
    public decimal Price { set; get; }
    public int Moq { set; get; }
    
    public int LeadTimeDays { set; get; }
    public DateTime UpdateDate { set; get; }

    public List<OfferPriceTier> PriceTiers { get; set; } = [];

    public decimal PriceForQty(int qty)
    {
        if (qty <= 0) return 0;

        if (PriceTiers is { Count:0 })
            if (qty >= Moq)
                return Price;
            else
                throw new ArgumentOutOfRangeException(nameof(qty));

        return PriceTiers
            .OrderBy(t => t.MinQty)
            .First(t => qty >= t.MinQty && (t.MaxQty is null || qty <= t.MaxQty))
            .Price;
    }
}

public sealed class OfferPriceTier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int MinQty { get; set; }
    public int? MaxQty { get; set; }
    public decimal Price { get; set; }
}