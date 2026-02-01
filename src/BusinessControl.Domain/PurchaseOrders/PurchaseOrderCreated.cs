namespace BusinessControl.Domain.Events;

public record PurchaseOrderCreated(Guid Id, Guid SupplierId);