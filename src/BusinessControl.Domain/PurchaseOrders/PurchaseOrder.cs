using BusinessControl.Domain.Aggregates;
using BusinessControl.Shared.Enums;

namespace BusinessControl.Domain.PurchaseOrders;

public class PurchaseOrder : AggregateRoot
{
    public OrderStatus Status { get; private set; }
    public Guid SupplierId { get; private set; } 

    public void Apply(PurchaseOrderCreated @event)
    {
        Id = @event.Id;
        SupplierId = @event.SupplierId;
        Status = OrderStatus.Draft;
    }

    public static PurchaseOrder Create(Guid id, Guid supplierId)
    {
        var order = new PurchaseOrder();
        order.ApplyEvent(new PurchaseOrderCreated(id, supplierId));
        return order;
    }
}