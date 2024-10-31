using Azure.Storage.Files.DataLake;
using OneLakeWriter;

var uri = new Uri("https://onelake.dfs.fabric.microsoft.com/mpiotfabric/");
var client = new DataLakeFileSystemClient(uri, Helper.ClientSecretCredential());

var directoryClient = client.GetDirectoryClient("Telemetries.lakehouse");

var paths = directoryClient.GetPaths();
foreach(var item in paths)
{
    Console.WriteLine(item.Name);
}

Console.ReadLine();

// https://learn.microsoft.com/en-us/fabric/data-engineering/lakehouse-api#load-to-tables-api-request


//var options = new ParquetOptions { TreatByteArrayAsString = true };
//var reader = new ParquetReader(response, options);

//var i = 0;
//var table = reader.ReadAsTable();
//foreach (var row in table)
//{
//    Console.WriteLine(row.Values);
//    //Console.Write($"{i++} ][ ");
//    //for (var j = 0; j < row.Length; j++)
//    //{
//    //    Console.Write(row[j]);
//    //    Console.Write(" // ");
//    //}
//    //Console.WriteLine();
//}
//Console.ReadLine();