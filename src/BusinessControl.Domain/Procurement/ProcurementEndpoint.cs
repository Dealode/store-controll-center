using BusinessControl.Domain.Procurement.Calculator;
using BusinessControl.Shared.Procurement;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace BusinessControl.Domain.Procurement;

public static class ProcurementEndpoint
{
    public static IEndpointRouteBuilder MapProcurement(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ProcurementRoutes.GroupPrefix).WithTags("Procurement");

        ProcessProductRequests(group);
        ProcessVendorsRequests(group);
        ProcessOffersRequests(group);

        group.MapPost(ProcurementRoutes.Calc.RelativeLanded, async (
            [FromServices] IQuerySession query,
            [FromBody] LandedCostRequest req) =>
        {
            var offer = await query.LoadAsync<VendorOfferDto>(req.OfferId);
            if (offer is null) return Results.NotFound("Offer not found.");

            var result = LandedCostCalculator.Calculate(offer, req);
            return Results.Ok(result);
        });

        return app;
    }

    private static void ProcessProductRequests(RouteGroupBuilder group)
    {
        group.MapGet(ProcurementRoutes.Products.RelativeBase, async (
            [FromServices] IQuerySession query) =>
        {
            var items = await query
                .Query<ProductDto>()
                .OrderBy(x => x.Name)
                .ToListAsync();
            
            return Results.Ok(items);
        });
        
        group.MapGet(ProcurementRoutes.Products.RelativeById, async (
            [FromServices] IQuerySession query,
            Guid id) =>
        {
            var product = await query
                .LoadAsync<ProductDto>(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        });
        
        group.MapPost(ProcurementRoutes.Products.RelativeBase, async (
            [FromServices] IDocumentSession session,
            [FromBody] ProductDto productDto) =>
        {
            if (productDto.Id == Guid.Empty) productDto.Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(productDto.Name)) return Results.BadRequest("Product.Name is required.");
            
            session.Store(productDto);
            await session.SaveChangesAsync();
            
            return Results.Created($"/api/procurement/products/{productDto.Id}", productDto);
        });

        group.MapPut(ProcurementRoutes.Products.RelativeById, async (
            [FromServices] IDocumentSession session,
            Guid id,
            [FromBody] ProductDto updated) =>
        {
            var existing = await session.LoadAsync<ProductDto>(id);
            if (existing is null) return Results.NotFound();

            existing.Id = id;
            session.Store(updated);
            await session.SaveChangesAsync();

            return Results.Ok(updated);
        });

        group.MapDelete(ProcurementRoutes.Products.RelativeById, async (
            [FromServices] IDocumentSession session,
            Guid id) =>
        {
            session.Delete<ProductDto>(id);
            await session.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static void ProcessVendorsRequests(RouteGroupBuilder group)
    {
        group.MapGet(ProcurementRoutes.Vendors.RelativeBase, async (
            [FromServices] IQuerySession query) =>
        {
            var items = await query.Query<VendorDto>().OrderBy(x => x.Name).ToListAsync();
            return Results.Ok(items);
        });
        
        group.MapGet(ProcurementRoutes.Vendors.RelativeById, async (
            [FromServices] IQuerySession query,
            Guid id) =>
        {
            var vendor = await query.LoadAsync<VendorDto>(id);
            return vendor is null ? Results.NotFound() : Results.Ok(vendor);
        });

        group.MapPost(ProcurementRoutes.Vendors.RelativeBase, async (
            [FromServices] IDocumentSession session,
            [FromBody] VendorDto vendorDto) =>
        {
            if (vendorDto.Id == Guid.Empty) vendorDto.Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(vendorDto.Name)) return Results.BadRequest("Vendor.Name is required.");

            session.Store(vendorDto);
            await session.SaveChangesAsync();
            return Results.Created($"/api/procurement/vendors/{vendorDto.Id}", vendorDto);
        });
        
        group.MapPut(ProcurementRoutes.Vendors.RelativeById, async (
            [FromServices] IDocumentSession session,
            Guid id,
            [FromBody] VendorDto updated) =>
        {
            var existing = await session.LoadAsync<VendorDto>(id);
            if (existing is null) return Results.NotFound();

            updated.Id = id;
            session.Store(updated);
            await session.SaveChangesAsync();
            return Results.Ok(updated);
        });
        
        group.MapDelete(ProcurementRoutes.Vendors.RelativeById, async (
            [FromServices] IDocumentSession session, Guid id) =>
        {
            session.Delete<VendorDto>(id);
            await session.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static void ProcessOffersRequests(RouteGroupBuilder group)
    {
        group.MapGet(ProcurementRoutes.Offers.RelativeBase, async (
            [FromServices] IQuerySession query,
            [FromQuery] Guid? productId,
            [FromQuery] Guid? vendorId) =>
        {
            var q = query.Query<VendorOfferDto>().AsQueryable();

            if (productId is not null && productId != Guid.Empty)
                q = q.Where(x => x.ProductId == productId);

            if (vendorId is not null && vendorId != Guid.Empty)
                q = q.Where(x => x.VendorId == vendorId);

            var items = await q.OrderByDescending(x => x.UpdateDate).ToListAsync();
            return Results.Ok(items);
        });
        
        group.MapGet(ProcurementRoutes.Offers.RelativeById, async (
            [FromServices] IQuerySession query,
            Guid id) =>
        {
            var offer = await query.LoadAsync<VendorOfferDto>(id);
            return offer is null ? Results.NotFound() : Results.Ok(offer);
        });

        group.MapPost(ProcurementRoutes.Offers.RelativeBase, async (
            [FromServices] IDocumentSession session,
            [FromBody] VendorOfferDto offerDto) =>
        {
            if (offerDto.Id == Guid.Empty) offerDto.Id = Guid.NewGuid();
            if (offerDto.ProductId == Guid.Empty) return Results.BadRequest("Offer.ProductId is required.");
            if (offerDto.VendorId == Guid.Empty) return Results.BadRequest("Offer.VendorId is required.");
            if (offerDto.Moq <= 0) return Results.BadRequest("Offer.Moq must be > 0.");

            if (offerDto.PriceTiers.Count == 0 && offerDto.Price <= 0)
                return Results.BadRequest("Offer must have Price > 0 or at least one PriceTier.");

            foreach (var t in offerDto.PriceTiers)
            {
                if (t.MinQty <= 0) return Results.BadRequest("PriceTier.MinQty must be > 0.");
                if (t.MaxQty is not null && t.MaxQty < t.MinQty)
                    return Results.BadRequest("PriceTier.MaxQty must be >= MinQty.");
                if (t.Price <= 0) return Results.BadRequest("PriceTier.Price must be > 0.");
            }

            offerDto.UpdateDate = DateTime.UtcNow;

            session.Store(offerDto);
            await session.SaveChangesAsync();
            return Results.Created($"/api/procurement/offers/{offerDto.Id}", offerDto);
        });
        
        group.MapPut(ProcurementRoutes.Offers.RelativeById, async (
            [FromServices] IDocumentSession session,
            Guid id,
            [FromBody] VendorOfferDto updated) =>
        {
            var existing = await session.LoadAsync<VendorOfferDto>(id);
            if (existing is null) return Results.NotFound();

            updated.Id = id;
            updated.UpdateDate = DateTime.UtcNow;

            session.Store(updated);
            await session.SaveChangesAsync();
            return Results.Ok(updated);
        });
        
        group.MapDelete(ProcurementRoutes.Offers.RelativeById, async ([FromServices] IDocumentSession session, Guid id) =>
        {
            session.Delete<VendorOfferDto>(id);
            await session.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}