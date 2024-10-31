using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using ReadFabricSqlEndpoint.Database;

namespace ReadFabricSqlEndpoint;

internal class Helper
{
    static string sqlServer = "";
    static string sqlDatabase = "Telemetries";
    static string connectionString = $"Data Source={sqlServer};Initial Catalog={sqlDatabase}";

    static string tenantId = "";
    static string clientId = "";
    static string clientSecret = "";
    static string authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

    public static async Task<string> GetAccessTokenAsync()
    {
        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithTenantId(tenantId)
            .WithClientSecret(clientSecret)
            .WithAuthority(authority)
        .Build();

        var accessTokenRequestBuilder = app.AcquireTokenForClient(
            new string[] { "https://database.windows.net//.default" }
        );
        var authenticationResult = await accessTokenRequestBuilder.ExecuteAsync();

        return authenticationResult.AccessToken;
    }

    public static async Task<SqlConnection> OpenSqlConnectionAsync()
    {
        var con = new SqlConnection(connectionString);
        con.AccessToken = await GetAccessTokenAsync();
        con.Open();
        return con;
    }

    public static TelemetriesDbContext GetDbContext()
    {
        var context = new TelemetriesDbContext();
        return context;
    }
}
