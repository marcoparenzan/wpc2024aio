using Parquet;
using System;
using Azure.Storage.Files.DataLake;
using Azure;
using Azure.Storage;

var credentials = new StorageSharedKeyCredential("tsaddata", "eKQOK5qekCyfHB2DhlS14NjKp55zef1tO3OcZ63vWF5+AC7WeuQ34dv4CytVuM28lAGNfpfRxs/VHkh+uCPERA==");
var client = new DataLakeFileClient(new Uri("https://tsaddata.blob.core.windows.net/iotdata00/2020/01/data_acme_2020_01_007955_0x10_a.parquet/part-00000-a67234fb-ae57-4e47-8228-f1e6eb9a2ad0-c000.snappy.parquet"), credentials);
var response = await client.OpenReadAsync();

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