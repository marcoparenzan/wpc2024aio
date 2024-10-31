using System;

namespace SynapseClient
{
    public class IoTEvent
    {
        public string DeviceId { get; set; }
        public DateTime EnqueuedTime { get; set; }
        public double Efficiency { get; set; }
        public double EnergyAmountkWh { get; set; }
        public double NominalVoltage { get; set; }
        public int PanelStatus { get; set; }
        public double PowerAmountKW { get; set; }
    }
}