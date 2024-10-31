// https://prodata.ie/2023/07/11/connecting-to-fabric-using-sqlclient/
// https://github.com/ProdataSQL/Fabric/

using Microsoft.Data.SqlClient;
using WarehouseEventHandling;
using System.Data;
using WarehouseEventHandling.Database;

await EntityFrameworkCore();
// await MicrosoftDataSqlClient();

// SCAFFOLDING
// https://github.com/dotnet/EntityFramework.Docs/issues/1138

static async Task MicrosoftDataSqlClient()
{
    //Set AAD Access Token, Open Connection, Run Queries and Disconnect
    var con = await Helper.OpenSqlConnectionAsync();
    var cmd = new SqlCommand();
    cmd.Connection = con;
    cmd.CommandType = CommandType.Text;
    cmd.CommandText = "SELECT count(*) FROM events.History";
    var res = cmd.ExecuteScalar();
    con.Close();

    Console.WriteLine(res);
}

static async Task EntityFrameworkCore()
{
    var context = Helper.GetDbContext();

    var items = context.Histories.Take(10).ToList();

    context.Add(new History
    {
        AssetName = $"{Guid.NewGuid()}",
        VariableName = "pppp",
        VariableType = "string",
        StringValue = "fdf"
    });
    context.SaveChanges();
}