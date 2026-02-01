namespace BusinessControl.Shared.Enums;

public enum OrderStatus
{
    None = 0,
    Draft = 1,
    PendingPayment = 2,
    Paid = 3,
    ShippingLocal = 4,
    ShippedLocal = 5,
    InTransit = 6,
    Delivered = 7,
    Received = 8,
    
    Cancelled = 10
}