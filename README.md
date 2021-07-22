# akka-asteroid-belt
Example how to use akka.net with and sharding in containerized setup

## What does this project?
The project simulates in a very simple way the movement of asteroids around a gravity pool. 
A asteroid is represented as actor in the akka cluster system. The asteriods are distributed via shards across avilable nodes in the cluster.
If two asteroids are crossing paths, the asteroid with the smaller weight is destroyed and cannot move anymore.
The state information about the asteroids are visible on a status dashboard (default page) and updated in realtime via signalR. 

## What is used?
* Akka.NET with Clustering: https://getakka.net/articles/clustering/cluster-overview.html
* Petabridge Cluster Management: https://cmd.petabridge.com/
* Asp.NetCore5: https://github.com/dotnet/aspnetcore
* Docker-Compose: https://docs.docker.com/compose/
* K8s: https://kubernetes.io/en/docs/home/

## Run project locally via DOCKER-COMPOSE and VS-Studio
* open https://github.com/ettenauer/akka-asteroid-belt/blob/main/AsteroidBelt.sln
* select docker-compose as start up project and press run 
* docker-compose will start three instances of AsteroidBelt.Web 
* browse https://localhost/ to see status dashboard

## Run project on K8s
* soon available
