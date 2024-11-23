namespace Wpc2024HMIApp.Utilities;

using Parquet.Serialization;
using Wpc2024HMIApp.Models;

public class DataSerializer
{
    public static async Task WriteCsvAsync(List<Sample> items, string csvFile)
    {
        using var csvStreamWriter = File.CreateText(csvFile);
        foreach (var item in items)
        {
            await csvStreamWriter.WriteLineAsync($"{item.Timestamp:yyyy-MM-dd HH:mm:ss.fff};{item.Temperature};{item.Pressure}");
        }
    }

    public static async Task WriteParquet(List<Sample> items, string parquetFile)
    {
        await ParquetSerializer.SerializeAsync(items, $"{parquetFile}");
    }
}
