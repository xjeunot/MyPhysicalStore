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
COPY ["ApiGateway/OcelotApiGw/OcelotApiGw.csproj", "ApiGateway/OcelotApiGw/"]
RUN dotnet restore "ApiGateway/OcelotApiGw/OcelotApiGw.csproj"
COPY . .
WORKDIR "/src/ApiGateway/OcelotApiGw"
RUN dotnet build "OcelotApiGw.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OcelotApiGw.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OcelotApiGw.dll"]