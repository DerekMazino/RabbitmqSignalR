#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["RabbitmqSignalRService/RabbitmqSignalRService.csproj", "RabbitmqSignalRService/"]
RUN dotnet restore "RabbitmqSignalRService/RabbitmqSignalRService.csproj"
COPY . .
WORKDIR "/src/RabbitmqSignalRService"
RUN dotnet build "RabbitmqSignalRService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitmqSignalRService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitmqSignalRService.dll"]
