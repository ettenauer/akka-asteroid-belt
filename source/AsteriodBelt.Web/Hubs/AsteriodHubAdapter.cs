using AsteriodBelt.Actors;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AsteriodBelt.Web.Hubs
{
    public class AsteriodHubAdapter
    {
        IHubContext<AsteriodHub> hub;

        public AsteriodHubAdapter(IHubContext<AsteriodHub> hub)
        {
            this.hub = hub;
        }

        public Task PushStateAsync(AsteriodState state)
        {
            return WriteMessageAsync(new HubMessageEnvelope
            {
                Id = state.AsteriodId,
                Message = $"|ID: {state.AsteriodId} | X: {state.X} | Y: {state.Y} | Weight: {state.Weight} | Destroyed: {state.Destroyed}|"
            });
        }

        private Task WriteMessageAsync(HubMessageEnvelope message)
        {
            return hub.Clients.All.SendAsync("writeState", message);
        }

        private sealed class HubMessageEnvelope
        {
            public string Id { get; init; }

            public string Message { get; init; }
        }
    }
}
