using Akka.Actor;
using Akka.Cluster.Tools.PublishSubscribe;
using Akka.Event;
using System;

namespace AsteriodBelt.Actors
{
    public sealed class GravityActor : ReceiveActor
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();
        private ICancelable pulseScheduler;

        private class Pulse
        {
            public static readonly Pulse Instance = new Pulse();
            private Pulse() { }
        }

        public class Move
        {
            public static readonly Move Instance = new Move();
            private Move() { }
        }

        public GravityActor(IActorRef mediator)
        {
            Receive<Pulse>(async => mediator.Tell(new Publish(TopicNames.GravityForce, Move.Instance)));
        }

        protected override void PreStart()
        {
            pulseScheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5), Self, Pulse.Instance, ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            pulseScheduler?.Cancel();
        }
    }
}
