using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace OneLakeWriter;

internal class Helper
{
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

    public static TokenCredential ClientSecretCredential() => new ClientSecretCredential(tenantId, clientId, clientSecret);
}
