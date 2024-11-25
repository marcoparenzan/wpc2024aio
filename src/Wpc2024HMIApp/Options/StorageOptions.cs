namespace Wpc2024HMIApp.Options
{
    public sealed class StorageOptions
    {
        public string CsvContainerName { get; set; }
        public string ParquetContainerName { get; set; }
        public string ConnectionString { get; set; }
    }
}
