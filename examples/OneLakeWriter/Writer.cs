//using Parquet;
//using Parquet.Data;
//using Parquet.Schema;

//namespace DataCollectorLib;

//public class DataSerializer
//{
//    public async Task Parquet(string name, Sample[] aitems, Stream fileStream)
//    {
//        var items = aitems.SelectMany(xx => xx.Select(yy => (xx.Timestamp, yy.SignalName, yy.DataType, yy.IsArray, yy.Value)).ToArray()).ToArray();

//        var nameColumn = new DataColumn(
//            new DataField<string>("Name"),
//            items.Select(xx => name).ToArray());
//        var timestampColumn = new DataColumn(
//            new DataField<DateTime>("Timestamp"),
//            items.Select(xx => xx.Timestamp.UtcDateTime).ToArray());
//        var signalNameColumn = new DataColumn(
//            new DataField<string>("SignalName"),
//            items.Select(xx => xx.SignalName).ToArray());
//        var variableTypeColumn = new DataColumn(
//            new DataField<string>("VariableType"),
//            items.Select(xx => xx.DataType).ToArray());
//        var boolColumn = new DataColumn(
//            new DataField<bool?>("bool"),
//            items.Select(xx => xx.DataType == "bool" && !xx.IsArray ? (bool)xx.Value : (bool?)null).ToArray());
//        var boolArrayColumn = new DataColumn(
//            new DataField<string?>("boolArray"),
//            items.Select(xx => xx.DataType == "bool" && xx.IsArray ? ((string)xx.Value) : ((string)null)).ToArray());
//        var charColumn = new DataColumn(
//            new DataField<string?>("char"),
//            items.Select(xx => xx.DataType == "char" && !xx.IsArray ? (string)xx.Value : (string)null).ToArray());
//        var stringColumn = new DataColumn(
//            new DataField<string?>("string"),
//            items.Select(xx => xx.DataType == "char" && xx.IsArray ? (string)xx.Value : null).ToArray());
//        var ushortColumn = new DataColumn(
//            new DataField<ushort?>("uint"),
//            items.Select(xx => xx.DataType == "uint" ? (ushort)xx.Value : (ushort?)null).ToArray());
//        var uintColumn = new DataColumn(
//            new DataField<uint?>("udint"),
//            items.Select(xx => xx.DataType == "udint" ? (uint)xx.Value : (uint?)null).ToArray());
//        var ulongColumn = new DataColumn(
//            new DataField<ulong?>("ulint"),
//            items.Select(xx => xx.DataType == "ulint" ? (ulong)xx.Value : (ulong?)null).ToArray());

//        // create file schema
//        var schema = new ParquetSchema(
//            nameColumn.Field,
//            timestampColumn.Field,
//            signalNameColumn.Field,
//            variableTypeColumn.Field,
//            boolColumn.Field,
//            boolArrayColumn.Field,
//            charColumn.Field,
//            stringColumn.Field,
//            ushortColumn.Field,
//            uintColumn.Field,
//            ulongColumn.Field
//        );

//        using (ParquetWriter parquetWriter = await ParquetWriter.CreateAsync(schema, fileStream))
//        {
//            parquetWriter.CompressionMethod = CompressionMethod.Gzip;
//            //parquetWriter.CompressionLevel = 5;

//            // create a new row group in the file
//            using (ParquetRowGroupWriter groupWriter = parquetWriter.CreateRowGroup())
//            {
//                await groupWriter.WriteColumnAsync(nameColumn);
//                await groupWriter.WriteColumnAsync(timestampColumn);
//                await groupWriter.WriteColumnAsync(signalNameColumn);
//                await groupWriter.WriteColumnAsync(variableTypeColumn);
//                await groupWriter.WriteColumnAsync(boolColumn);
//                await groupWriter.WriteColumnAsync(boolArrayColumn);
//                await groupWriter.WriteColumnAsync(charColumn);
//                await groupWriter.WriteColumnAsync(stringColumn);
//                await groupWriter.WriteColumnAsync(ushortColumn);
//                await groupWriter.WriteColumnAsync(uintColumn);
//                await groupWriter.WriteColumnAsync(ulongColumn);
//            }
//        }
//    }

//    public async Task CSV(string name, Sample[] aitems, Stream fileStream)
//    {
//        var items = aitems.SelectMany(xx => xx.Select(yy => (xx.Timestamp, yy.SignalName, yy.DataType, yy.IsArray, yy.Value)).ToArray()).ToArray();

//        using (var writer = new StreamWriter(fileStream))
//        {
//            foreach (var item in items)
//            {
//                writer.WriteLine($"{name};{item.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")};{item.SignalName};{item.DataType};{item.Value}");
//            }
//        }
//    }
//}