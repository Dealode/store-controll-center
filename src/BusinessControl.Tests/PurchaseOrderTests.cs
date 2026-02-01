using BusinessControl.Domain.Aggregates;
using BusinessControl.Domain.PurchaseOrders;
using BusinessControl.Shared.Enums;
using FluentAssertions;

namespace BusinessControl.Tests;

[TestFixture]
public class PurchaseOrderTests
{
    [Test]
    public void Create_ShouldRaise_PurchaseOrderCreated_Event()
    {
        // Arrange
        var id = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        
        // Act
        var order = PurchaseOrder.Create(id, supplierId);
        
        // Assert
        order.Id.Should().Be(id);
        order.SupplierId.Should().Be(supplierId);
        order.Status.Should().Be(OrderStatus.Draft);

        var events = order.GetUncommittedEvents();
        events.Should().ContainSingle()
            .Which.Should().BeOfType<PurchaseOrderCreated>();
    }
}