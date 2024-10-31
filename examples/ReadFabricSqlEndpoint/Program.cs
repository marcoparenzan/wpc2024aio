// https://prodata.ie/2023/07/11/connecting-to-fabric-using-sqlclient/
// https://github.com/ProdataSQL/Fabric/

using Microsoft.Data.SqlClient;
using ReadFabricSqlEndpoint;
using System.Data;

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
    cmd.CommandText = "SELECT count(*) FROM dbo.Thermometer01Over50";
    var res = cmd.ExecuteScalar();
    con.Close();

    Console.WriteLine(res);
}

static async Task EntityFrameworkCore()
{
    var context = Helper.GetDbContext();

    var items = context.Thermometer01s.Take(10).ToList();
}