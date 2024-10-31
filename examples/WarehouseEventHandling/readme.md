Server=ttj63cmsy5p7evlztqewiyjtvbm-f7blxb434ixuro5wjo7bz5tufm.datawarehouse.fabric.microsoft.com;Database=Telemetries

https://www.entityframeworktutorial.net/efcore/create-model-for-existing-database-in-ef-core.aspx

Scaffold-DbContext -Connection "Data Source=tj63cmsy5p7evlztqewiyjtvbm-f7blxb434ixuro5wjo7bz5tufm.datawarehouse.fabric.microsoft.com;Initial Catalog=Messages" -Provider Microsoft.EntityFrameworkCore.SqlServer -OutputDir Database -Context MessagesDbContext -Force -NoOnConfiguring -Schema events
