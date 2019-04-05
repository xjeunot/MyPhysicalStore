FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5001

FROM microsoft/dotnet:2.2-sdk AS build
#Download Package Enterprise CA Security : Start Section
WORKDIR /usr/local/share/ca-certificates
COPY Security/cert_rootcagfi.crt cert_rootcagfi.crt
RUN update-ca-certificates
#Download Package Enterprise CA Security : End Section
WORKDIR /src
COPY ["Services/Customer/Customer.API/Customer.API.csproj", "Services/Customer/Customer.API/"]
COPY ["BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
RUN dotnet restore "Services/Customer/Customer.API/Customer.API.csproj"
COPY . .
WORKDIR "/src/Services/Customer/Customer.API"
RUN dotnet build "Customer.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Customer.API.csproj" -c Release -o /app

FROM base AS final
#Download Package Personnal CA Security : Start Section
WORKDIR /usr/local/share/ca-certificates
COPY Security/Xjeunot_CA.crt Xjeunot_CA.crt
RUN update-ca-certificates
#Download Package Personnal CA Security : End Section
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Customer.API.dll"]
