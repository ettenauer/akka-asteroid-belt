using Akka.Cluster.Sharding;

namespace AsteriodBelt.Actors
{
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as MessageEnvelope)?.EntityId.ToString();

        public string ShardId(object message) => (message as MessageEnvelope)?.ShardId.ToString();

        public object EntityMessage(object message) => (message as MessageEnvelope)?.Message;
    }
}
