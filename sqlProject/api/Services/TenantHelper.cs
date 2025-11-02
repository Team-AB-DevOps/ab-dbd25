namespace api.Services;

public static class TenantHelper
{
    public static string GetTenant(HttpRequest request)
    {
        var tenant = request.Headers["X-tenant"].ToString();
        return string.IsNullOrWhiteSpace(tenant) ? "sql" : tenant;
    }
}
