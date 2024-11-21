using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace Wpc2024HMIApp.Helpers;

internal class AzureHelper
{
    public static async Task<string> GetAccessTokenAsync(string tenantId, string clientId, string clientSecret)
    {
        string authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

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

    public static TokenCredential ClientSecretCredential(string tenantId, string clientId, string clientSecret) => new ClientSecretCredential(tenantId, clientId, clientSecret);
}

