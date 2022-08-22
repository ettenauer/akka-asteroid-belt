# akka-asteroid-belt
Example how to use akka.net with sharding in containerized setup

## What does this project?
The project simulates in a very simple way the movement of asteroids around a gravity pool. 
A asteroid is represented as actor in the akka cluster system. The asteriods are distributed via shards across available nodes in the cluster.
If two asteroids are crossing paths, the asteroid with the smaller weight is destroyed and cannot move anymore.
The state information about the asteroids are visible on a status dashboard (default page) and updated in realtime via signalR. 

In case of a failover the moved asteroid actors are recreated with their inital state since the project doesn't use a persistent storage.

## What is used?
* Akka.NET with Clustering: https://getakka.net/articles/clustering/cluster-overview.html
* Petabridge Cluster Management: https://cmd.petabridge.com/ -> used for failover tests
* Asp.NetCore6: https://github.com/dotnet/aspnetcore
* Docker-Compose: https://docs.docker.com/compose/
* K8s: https://kubernetes.io/en/docs/home/

## Run project locally via DOCKER-COMPOSE and VS-Studio
* open https://github.com/ettenauer/akka-asteroid-belt/blob/main/AsteroidBelt.sln
* select docker-compose as start up project and press run 
* docker-compose will start three instances of AsteroidBelt.Web 
* browse https://localhost/ to see status dashboard

## Run project on K8s
* install K8s and Helm
* activate ingress controller
* kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v0.47.0/deploy/static/provider/cloud/deploy.yaml
* create secret able to access image repository
* kubectl create secret docker-registry private-docker-registry-cred --docker-server=https://ghcr.io --docker-username= --docker-password= --namespace=default -o yaml
* navigate to cloned repository
* helm install local ./k8s
* browse http://asteroidbelt.local/  (update hostfile to map asteroidbelt.local to 127.0.0.1)
