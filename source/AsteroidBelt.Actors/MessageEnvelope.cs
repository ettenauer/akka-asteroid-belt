namespace AsteroidBelt.Actors
{
    public sealed class MessageEnvelope
    {
        public int ShardId { get; init; }

        public int EntityId { get; init; }

        public object Message { get; init; }
    }
}
