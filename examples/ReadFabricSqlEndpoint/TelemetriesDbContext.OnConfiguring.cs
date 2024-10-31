using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ReadFabricSqlEndpoint.Database;

public partial class TelemetriesDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // https://stackoverflow.com/questions/54187241/ef-core-connection-to-azure-sql-with-managed-identity
        SqlConnection connection = Helper.OpenSqlConnectionAsync().Result;
        optionsBuilder.UseSqlServer(connection);
    }
}