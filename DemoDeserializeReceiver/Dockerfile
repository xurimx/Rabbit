#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["DemoDeserializeReceiver/DemoDeserializeReceiver.csproj", "DemoDeserializeReceiver/"]
RUN dotnet restore "DemoDeserializeReceiver/DemoDeserializeReceiver.csproj"
COPY . .
WORKDIR "/src/DemoDeserializeReceiver"
RUN dotnet build "DemoDeserializeReceiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoDeserializeReceiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoDeserializeReceiver.dll"]