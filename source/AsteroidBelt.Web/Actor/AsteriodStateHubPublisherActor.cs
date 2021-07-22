using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using AsteroidBelt.Actors;
using AsteroidBelt.Web.Hubs;
using System;

namespace AsteroidBelt.Web.Actor
{
    public class AsteriodStateHubPublisherActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        public AsteriodStateHubPublisherActor(AsteroidHubAdapter hub, IActorRef mediator)
        {
            mediator.Tell(new Subscribe(TopicNames.AsteriodState, Self));

            Receive<SubscribeAck>(ack =>
            {
                if (ack.Subscribe.Topic == TopicNames.AsteriodState
                    && ack.Subscribe.Ref.Equals(Self)
                    && ack.Subscribe.Group == null)
                {
                    logger.Info($"subscribing asteriod {Self.Path.Name} to topic {ack.Subscribe.Topic}");
                }
            });

            Receive<UnsubscribeAck>(ack =>
            {
                logger.Info($"unsubscribing asteriod {Self.Path.Name} from topic {ack.Unsubscribe.Topic}");
            });

            ReceiveAsync<AsteroidState>(async state =>
            {
                try
                {
                    logger.Info($"reiceved state asteriod: {state.AsteroidId} x: {state.X} y: {state.Y}");

                    await hub.PushStateAsync(state).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error while writing to hub");
                }
            });
        }
    }
}
