using Akka.Actor;
using System;
using System.Collections.Generic;

namespace AsteroidBelt.Actors
{
    /// <summary>
    /// Intended to be a Cluster Singleton. Responsible for ensuring there's at least one instance of asteriod is present in cluster.
    /// </summary>
    public class AsteroidInitiaterActor : ReceiveActor
    {
        private class Heartbeat
        {
            public static readonly Heartbeat Instance = new Heartbeat();
            private Heartbeat() { }
        }
 
        private ICancelable heartbeatScheduler;

        public AsteroidInitiaterActor(IEnumerable<int> asteriodIds, int numberOfShards, IActorRef asteriodProxy)
        {
            Receive<Heartbeat>(h =>
            {
                foreach (var id in asteriodIds)
                {
                    asteriodProxy.Tell(new MessageEnvelope { ShardId =  id % numberOfShards, EntityId = id, Message = new Ping { EntityId = id.ToString() } } );
                }
            });
        }

        protected override void PreStart()
        {
            heartbeatScheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30), Self, Heartbeat.Instance, ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            heartbeatScheduler?.Cancel();
        }

    }
}
