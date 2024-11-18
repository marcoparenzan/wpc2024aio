
using Wpc2024HMIApp.Models;

namespace Wpc2024HMIApp.Services
{
    public interface IHmiService
    {
        Task AddHmiValueAsync(string GroupName, string Name, object Value);
        Task SubscribeAsync(Action<HmiValue> action);
    }
}
