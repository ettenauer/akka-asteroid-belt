using AsteroidBelt.Actors;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AsteroidBelt.Web.Hubs;

public class AsteroidHubAdapter
{
    IHubContext<AsteroidHub> hub;

    public AsteroidHubAdapter(IHubContext<AsteroidHub> hub)
    {
        this.hub = hub;
    }

    public Task PushStateAsync(AsteroidState state)
    {
        return WriteMessageAsync(new HubMessageEnvelope
        {
            Id = state.AsteroidId,
            Message = $"|ID: {state.AsteroidId} | X: {state.X} | Y: {state.Y} | Weight: {state.Weight} | Destroyed: {state.Destroyed}|"
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

