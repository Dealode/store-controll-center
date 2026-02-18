using BusinessControl.Domain.Procurement.Calculator;
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
        var group = app.MapGroup("api/procurement").WithTags("Procurement");

        ProcessProductRequests(group);
        ProcessVendorsRequests(group);
        ProcessOffersRequests(group);

        group.MapPost("/calc/landed", async (
            [FromServices] IQuerySession query,
            [FromBody] LandedCostRequest req) =>
        {
            var offer = await query.LoadAsync<VendorOffer>(req.OfferId);
            if (offer is null) return Results.NotFound("Offer not found.");

            var result = LandedCostCalculator.Calculate(offer, req);
            return Results.Ok(result);
        });

        return app;
    }

    private static void ProcessProductRequests(RouteGroupBuilder group)
    {
        group.MapPost("/products", async (
            [FromServices] IDocumentSession session,
            [FromBody] Product product) =>
        {
            if (product.Id == Guid.Empty) product.Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(product.Name)) return Results.BadRequest("Product.Name is required.");
            
            session.Store(product);
            await session.SaveChangesAsync();
            
            return Results.Created($"/api/procurement/products/{product.Id}", product);
        });
    }

    private static void ProcessVendorsRequests(RouteGroupBuilder group)
    {
        group.MapGet("/vendors", async (
            [FromServices] IDocumentSession session,
            [FromBody] Vendor vendor) =>
        {
            if (vendor.Id == Guid.Empty) vendor.Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(vendor.Name)) return Results.BadRequest("Vendor.Name is required.");

            session.Store(vendor);
            await session.SaveChangesAsync();
            return Results.Created($"/api/procurement/vendors/{vendor.Id}", vendor);
        });

        group.MapGet("/vendors/{id:guid}", async (
            [FromServices] IQuerySession query,
            Guid id) =>
        {
            var vendor = await query.LoadAsync<Vendor>(id);
            return vendor is null ? Results.NotFound() : Results.Ok(vendor);
        });
    }

    private static void ProcessOffersRequests(RouteGroupBuilder group)
    {
        group.MapPost("/offers", async (
            [FromServices] IDocumentSession session,
            [FromBody] VendorOffer offer) =>
        {
            if (offer.Id == Guid.Empty) offer.Id = Guid.NewGuid();
            if (offer.ProductId == Guid.Empty) return Results.BadRequest("Offer.ProductId is required.");
            if (offer.VendorId == Guid.Empty) return Results.BadRequest("Offer.VendorId is required.");
            if (offer.Moq <= 0) return Results.BadRequest("Offer.Moq must be > 0.");

            // Мінімальна валідація tiers
            if (offer.PriceTiers.Count == 0)
                return Results.BadRequest("Offer must have at least one PriceTier.");

            foreach (var t in offer.PriceTiers)
            {
                if (t.MinQty <= 0) return Results.BadRequest("PriceTier.MinQty must be > 0.");
                if (t.MaxQty is not null && t.MaxQty < t.MinQty) return Results.BadRequest("PriceTier.MaxQty must be >= MinQty.");
                if (t.Price <= 0) return Results.BadRequest("PriceTier.UnitPrice must be > 0.");
            }

            session.Store(offer);
            await session.SaveChangesAsync();
            return Results.Created($"/api/procurement/offers/{offer.Id}", offer);
        });
        
        group.MapGet("/offers/{id:guid}", async (
            [FromServices] IQuerySession query,
            Guid id) =>
        {
            var offer = await query.LoadAsync<VendorOffer>(id);
            return offer is null ? Results.NotFound() : Results.Ok(offer);
        });
        
        group.MapPost("/offers/{id:guid}/tiers", async (
            [FromServices] IDocumentSession session,
            Guid id,
            [FromBody] OfferPriceTier tier) =>
        {
            var offer = await session.LoadAsync<VendorOffer>(id);
            if (offer is null) return Results.NotFound();

            if (tier.MinQty <= 0) return Results.BadRequest("MinQty must be > 0.");
            if (tier.MaxQty is not null && tier.MaxQty < tier.MinQty) return Results.BadRequest("MaxQty must be >= MinQty.");
            if (tier.Price <= 0) return Results.BadRequest("UnitPrice must be > 0.");

            offer.PriceTiers.Add(tier);
            session.Store(offer);
            await session.SaveChangesAsync();
            return Results.Ok(offer);
        });
    }
}