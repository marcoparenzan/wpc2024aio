using System;
using System.Data.SqlClient;
using Dapper;
using SynapseClient;

var conn = new SqlConnection("");
conn.Open();

var result = conn.Query<IoTEvent>("SELECT [deviceId],[enqueuedTime],[Efficiency],[EnergyAmountkWh],[NominalVoltage],[PanelStatus],[PowerAmountKW] FROM [iotdata4]");
foreach (var xx in result)
{
    Console.WriteLine($"{xx.DeviceId}\t{xx.EnqueuedTime}\t{xx.Efficiency}\t{xx.EnergyAmountkWh}\t{xx.NominalVoltage}\t{xx.PanelStatus}");
}

conn.Close();