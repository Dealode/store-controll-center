namespace BusinessControl.Domain.Procurement.Cargos;

public sealed class Cargo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class DeliveryCostResult
{
    public Guid Id { get; set; }
    public Guid CargoId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Volume { get; set; }
    public decimal Weight { get; set; }
    public decimal Price { get; set; }
}

public sealed class DeliveryVariant
{
    public Guid Id { get; set; }
    public Guid CargoId { get; set; }
    public float MinimalWeight { set; get; }
    public int LeadTimeDays { get; set; }
}