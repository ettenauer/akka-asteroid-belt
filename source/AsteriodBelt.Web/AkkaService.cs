using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Cluster.Tools.Singleton;
using Akka.Configuration;
using Akka.DependencyInjection;
using Akka.Util;
using AsteriodBelt.Actors;
using AsteriodBelt.Web.Actor;
using Microsoft.Extensions.Hosting;
using Petabridge.Cmd.Cluster;
using Petabridge.Cmd.Cluster.Sharding;
using Petabridge.Cmd.Host;
using Petabridge.Cmd.Remote;

namespace AsteriodBelt.Web
{
    public class AkkaService : IHostedService
    {
        private const int NumberOfShards = 2;
        //Note: these asteriods should be distributed within the akka cluster
        private static int[] AsteriodIds = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        private readonly IHostApplicationLifetime lifetime;
        private readonly IServiceProvider provider;
        private ActorSystem actorSystem;

        public AkkaService(IServiceProvider provider, IHostApplicationLifetime lifetime)
        {
            this.provider = provider;
            this.lifetime = lifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var dependencyProvider = DependencyResolverSetup.Create(provider);
            var hocon = ConfigurationFactory.ParseString(File.ReadAllText("app.conf")).BootstrapFromDocker();   
            var bootstrap = BootstrapSetup.Create().WithConfig(hocon);
            var di = DependencyResolverSetup.Create(provider);
            var actorSystemSetup = bootstrap.And(di);
          
            actorSystem = ActorSystem.Create("asteriodbelt", actorSystemSetup);

            var mediator = Akka.Cluster.Tools.PublishSubscribe.DistributedPubSub.Get(actorSystem).Mediator;

            //Note: actor to publish state updates on SignalR
            var publisherProps = DependencyResolver.For(actorSystem).Props<AsteriodStateHubPublisherActor>(mediator);
            actorSystem.ActorOf(publisherProps, nameof(AsteriodStateHubPublisherActor));

            var shardProxy = ClusterSharding.Get(actorSystem).Start(
                   typeName: typeof(AsteroidActor).Name,
                   entityProps: Props.Create<AsteroidActor>(mediator),
                   settings: ClusterShardingSettings.Create(actorSystem).WithRole("asteriod"),
                   messageExtractor: new MessageExtractor());

            actorSystem.ActorOf(ClusterSingletonManager.Props(Props.Create(() => new AsteroidInitiaterActor(AsteriodIds, NumberOfShards, shardProxy)),
                ClusterSingletonManagerSettings.Create(actorSystem)
                    .WithRole("asteriod")
                    .WithSingletonName("asteriodHeartbeat")), "asteriodHeartbeat");

            actorSystem.ActorOf(ClusterSingletonManager.Props(Props.Create(() => new GravityActor(mediator)),
                ClusterSingletonManagerSettings.Create(actorSystem)
                    .WithRole("gravity")
                    .WithSingletonName("gravity-force")), "gravity-force");

            actorSystem.WhenTerminated.ContinueWith(tr =>
            {
                lifetime.StopApplication();
            });

            // start Petabridge.Cmd (for external monitoring / supervision)
            var pbm = PetabridgeCmd.Get(actorSystem);
            pbm.RegisterCommandPalette(ClusterCommands.Instance);
            pbm.RegisterCommandPalette(ClusterShardingCommands.Instance);
            pbm.RegisterCommandPalette(RemoteCommands.Instance);
            pbm.Start();

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await actorSystem.Terminate().ConfigureAwait(false);
        }
    }
}
