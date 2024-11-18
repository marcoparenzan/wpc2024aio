using MQTTnet.Client;
using MQTTnet;
using Microsoft.Extensions.Options;
using Wpc2024HMIApp.Options;
using MQTTnet.Server;
using Wpc2024HMIApp.Models;
using System.Threading.Channels;

namespace Wpc2024HMIApp.Services
{
    public class HmiService : IHmiService
    {
        private readonly Channel<HmiValue> HmiValueChannel = Channel.CreateUnbounded<HmiValue>();
        private readonly List<Task> Subscriptions = [];

        public async Task AddHmiValueAsync(string GroupName, string Name, object Value)
        {
            await HmiValueChannel.Writer.WriteAsync(new HmiValue(GroupName, Name, Value));
        }

        public Task SubscribeAsync(Action<HmiValue> action)
        {
            Subscriptions.Add(Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var value = await HmiValueChannel.Reader.ReadAsync();
                    action(value);
                }
            }            
            ));

            return Task.CompletedTask;
        }
    }
}
