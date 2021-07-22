#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

# should be a comma-delimited list
ENV CLUSTER_SEEDS "[]"
ENV CLUSTER_IP ""
ENV CLUSTER_PORT "4053"

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["source/AsteriodBelt.Web/AsteriodBelt.Web.csproj", "source/AsteriodBelt.Web/"]
COPY ["source/AsteriodBelt.Actors/AsteriodBelt.Actors.csproj", "source/AsteriodBelt.Actors/"]
RUN dotnet restore "source/AsteriodBelt.Web/AsteriodBelt.Web.csproj"
COPY . .
WORKDIR "/src/source/AsteriodBelt.Web"
RUN dotnet build "AsteriodBelt.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AsteriodBelt.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 9110 # 9110 - Petabridge.Cmd
EXPOSE 4053 # 4053 - Akka.Cluster
EXPOSe 80
EXPOSE 443

ENTRYPOINT ["dotnet", "AsteriodBelt.Web.dll"]
