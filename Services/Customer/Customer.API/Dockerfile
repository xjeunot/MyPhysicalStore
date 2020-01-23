#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
#Download Package Personnal CA Security : Start Section
WORKDIR /usr/local/share/ca-certificates
COPY Security/Xjeunot_CA.crt Xjeunot_CA.crt
RUN update-ca-certificates
#Download Package Personnal CA Security : End Section
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Customer/Customer.API/Customer.API.csproj", "Services/Customer/Customer.API/"]
COPY ["BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/EventBusAzure/EventBusAzure.csproj", "BuildingBlocks/EventBusAzure/"]
RUN dotnet restore "Services/Customer/Customer.API/Customer.API.csproj"
COPY . .
WORKDIR "/src/Services/Customer/Customer.API"
RUN dotnet build "Customer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.API.dll"]