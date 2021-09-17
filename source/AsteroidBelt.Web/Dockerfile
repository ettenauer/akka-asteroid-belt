#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

ENV CLUSTER_SEEDS "[]"
ENV CLUSTER_IP ""
ENV CLUSTER_PORT "4053"

EXPOSE 9110
EXPOSE 4053
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["source/AsteroidBelt.Web/AsteroidBelt.Web.csproj", "source/AsteroidBelt.Web/"]
COPY ["source/AsteroidBelt.Actors/AsteroidBelt.Actors.csproj", "source/AsteroidBelt.Actors/"]
RUN dotnet restore "source/AsteroidBelt.Web/AsteroidBelt.Web.csproj"
COPY . .
WORKDIR "/src/source/AsteroidBelt.Web"
RUN dotnet build "AsteroidBelt.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AsteroidBelt.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AsteroidBelt.Web.dll"]