#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DemoApp.Receiver/DemoApp.Receiver.csproj", "DemoApp.Receiver/"]
RUN dotnet restore "DemoApp.Receiver/DemoApp.Receiver.csproj"
COPY . .
WORKDIR "/src/DemoApp.Receiver"
RUN dotnet build "DemoApp.Receiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoApp.Receiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApp.Receiver.dll"]