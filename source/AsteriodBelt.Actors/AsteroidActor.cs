using Akka.Actor;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using System;

namespace AsteriodBelt.Actors
{
    public sealed class AsteroidActor : ReceiveActor
    {
        private const int DestructionRadius = 5;

        private readonly ILoggingAdapter logger = Context.GetLogger();
        private readonly int weight;

        public AsteroidActor(IActorRef mediator)
        {
            this.weight = new Random(Guid.NewGuid().GetHashCode()).Next(20, 50);
            var motion = new AsteroidMotion();

            AliveBehavior(motion, mediator);
        }

        private void AliveBehavior(AsteroidMotion motion, IActorRef mediator)
        {
            DefaultBehavior(mediator);

            Receive<GravityActor.Move>(_ =>
            {
                var (x, y) = motion.Move();
                
                mediator.Tell(new Publish(TopicNames.AsteriodState,
                    new AsteriodState 
                    { 
                        AsteriodId = Self.Path.Name,
                        X = x,
                        Y = y,
                        Weight = weight,
                        Destroyed = false 
                    } ));
            });

            Receive<AsteriodState>(_ =>
            {
                if (_.AsteriodId != Self.Path.Name && _.Weight >= weight)
                {
                    var (x, y) = motion.CurrentPosition;

                    if (_.X <= x + DestructionRadius && _.X >= x - DestructionRadius &&
                        _.Y <= y + DestructionRadius && _.Y >= y - DestructionRadius)
                    {
                        logger.Info($"Asteriod {Self.Path.Name} is destroyed by {Sender.Path.Name}");

                        Become(() => DestroyedBehavior(motion, mediator));
                    }
                }
            });

            mediator.Tell(new Subscribe(TopicNames.GravityForce, Self));
            mediator.Tell(new Subscribe(TopicNames.AsteriodState, Self));

            Receive<SubscribeAck>(ack =>
            {
                if ((ack.Subscribe.Topic == TopicNames.GravityForce || ack.Subscribe.Topic == TopicNames.AsteriodState)
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
        }

        private void DestroyedBehavior(AsteroidMotion motion, IActorRef mediator)
        {
            DefaultBehavior(mediator);

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

            Receive<GravityActor.Move>(_ =>
            {
                var (x, y) = motion.CurrentPosition;

                mediator.Tell(new Publish(TopicNames.AsteriodState,
                    new AsteriodState 
                    {
                        AsteriodId = Self.Path.Name,
                        X = x,
                        Y = y,
                        Weight = weight,
                        Destroyed = true 
                    }));
            });
        }

        private void DefaultBehavior(IActorRef mediator)
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(60));
            Receive<ReceiveTimeout>(_ =>
            {
                Context.Parent.Tell(new Passivate(PoisonPill.Instance));
            });

            Receive<Ping>(_ => logger.Debug("pinged via {0}", Sender));

            Receive<Terminated>(_ =>
            {
                mediator.Tell(new Unsubscribe(TopicNames.GravityForce, Self));

                logger.Info($"Asteriod {Self.Path.Name} terminated");
            });
        }
    }
}
