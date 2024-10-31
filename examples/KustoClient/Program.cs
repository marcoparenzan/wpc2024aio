
using Kusto.Data;
using Kusto.Data.Net.Client;

var kustoUri = ""; //  https://trdhqrj1yvbjna998nrt41.z7.kusto.fabric.microsoft.com";
var connectionStringBuilder = new KustoConnectionStringBuilder(kustoUri, "")
    //.WithAadUserPromptAuthentication()
    .WithAadApplicationKeyAuthentication("", "", "")
;
    
var client = KustoClientFactory.CreateCslQueryProvider(connectionStringBuilder.ConnectionString);

var reader = client.ExecuteQuery("", null);

while (reader.Read())
{
    Console.WriteLine($"XX={reader.GetInt64(0)}");
}