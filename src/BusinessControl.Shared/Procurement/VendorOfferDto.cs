using BusinessControl.Shared.Procurement.Enums;

namespace BusinessControl.Shared.Procurement;

public sealed class VendorOfferDto
{
    public Guid Id { set; get; }
    
    public Guid ProductId { set; get; }
    public Guid VendorId { set; get; }
    
    public CurrencyCode Currency { set; get; }
    public decimal Price { set; get; }
    public int Moq { set; get; }
    
    public int LeadTimeDays { set; get; }
    public DateTime UpdateDate { set; get; }

    public List<OfferPriceTierDto> PriceTiers { get; set; } = [];

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