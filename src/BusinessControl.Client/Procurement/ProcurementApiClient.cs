using System.Net.Http.Json;
using BusinessControl.Shared.Procurement;

namespace BusinessControl.Client.Procurement;

public sealed class ProcurementApiClient(HttpClient httpClient)
{
    public async Task<List<ProductDto>> GetProductsAsync() =>
        await httpClient
            .GetFromJsonAsync<List<ProductDto>>(ProcurementRoutes.Products.Base)
        ?? [];

    public async Task<ProductDto?> GetProductAsync(Guid id) =>
        await httpClient
            .GetFromJsonAsync<ProductDto>(ProcurementRoutes.Products.GetById(id));

    public async Task<ProductDto?> CreateProductAsync(ProductDto dto)
    {
        var resp = await httpClient
            .PostAsJsonAsync(ProcurementRoutes.Products.Base, dto);
        
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ProductDto>())!;
    }
    
    public async Task UpdateProductAsync(Guid id, ProductDto dto)
    {
        var resp = await httpClient
            .PutAsJsonAsync(ProcurementRoutes.Products.GetById(id), dto);
        resp.EnsureSuccessStatusCode();
    }
    
    public async Task DeleteProductAsync(Guid id)
    {
        var resp = await httpClient
            .DeleteAsync(ProcurementRoutes.Products.GetById(id));
        resp.EnsureSuccessStatusCode();
    }
}