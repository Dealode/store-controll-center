namespace BusinessControl.Shared.Procurement;

public static class ProcurementRoutes
{
    public const string GroupPrefix = "api/procurement";
    
    public static class Products
    {
        public const string RelativeBase = "/products";
        public const string RelativeById = "/products/{id:guid}";

        public const string Base = $"{GroupPrefix}{RelativeBase}";
        public const string ById = $"{GroupPrefix}{RelativeById}";

        public static string GetById(Guid id) => $"{Base}/{id}";
    }

    public static class Vendors
    {
        public const string RelativeBase = "/vendors";
        public const string RelativeById = "/vendors/{id:guid}";

        public const string Base = $"{GroupPrefix}{RelativeBase}";
        public const string ById = $"{GroupPrefix}{RelativeById}";

        public static string GetById(Guid id) => $"{Base}/{id}";
    }

    public static class Offers
    {
        public const string RelativeBase = "/offers";
        public const string RelativeById = "/offers/{id:guid}";

        public const string Base = $"{GroupPrefix}{RelativeBase}";
        public const string ById = $"{GroupPrefix}{RelativeById}";

        public static string GetById(Guid id) => $"{Base}/{id}";
    }

    public static class Calc
    {
        public const string RelativeLanded = "/calc/landed";
        public const string Landed = $"{GroupPrefix}{RelativeLanded}";
    }
}