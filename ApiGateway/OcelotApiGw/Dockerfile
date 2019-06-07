FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
#Download Package Personnal CA Security : Start Section
WORKDIR /usr/local/share/ca-certificates
COPY Security/Xjeunot_CA.crt Xjeunot_CA.crt
RUN update-ca-certificates
#Download Package Personnal CA Security : End Section
WORKDIR /app
EXPOSE 5001

FROM microsoft/dotnet:2.2-sdk AS build
#Download Package Enterprise CA Security : Start Section
WORKDIR /usr/local/share/ca-certificates
COPY Security/cert_rootcagfi.crt cert_rootcagfi.crt
RUN update-ca-certificates
#Download Package Enterprise CA Security : End Section
WORKDIR /src
COPY ["ApiGateway/OcelotApiGw/OcelotApiGw.csproj", "ApiGateway/OcelotApiGw/"]
RUN dotnet restore "ApiGateway/OcelotApiGw/OcelotApiGw.csproj"
COPY . .
WORKDIR "/src/ApiGateway/OcelotApiGw"
RUN dotnet build "OcelotApiGw.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OcelotApiGw.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OcelotApiGw.dll"]
