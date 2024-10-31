using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WarehouseEventHandling.Database;

namespace WarehouseEventHandling;

internal class Helper
{
    static string sqlServer = "";
    static string sqlDatabase = "Messages";
    static string connectionString = $"Data Source={sqlServer};Initial Catalog={sqlDatabase}";

    static string tenantId = "";
    static string clientId = "";
    static string clientSecret = "";
    static string authority = $"";

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

    public static MessagesDbContext GetDbContext()
    {
        var options = new DbContextOptions<MessagesDbContext>();
        var context = new MessagesDbContext(options);
        return context;
    }
}
