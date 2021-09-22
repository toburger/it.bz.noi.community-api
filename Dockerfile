FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
FROM mcr.microsoft.com/dotnet/sdk:5.0.102-alpine3.12-amd64 AS build
WORKDIR /src
COPY it.bz.noi.community-api/it.bz.noi.community-api.csproj it.bz.noi.community-api/
RUN dotnet restore it.bz.noi.community-api/it.bz.noi.community-api.csproj
COPY . .
WORKDIR /src/it.bz.noi.community-api
RUN dotnet build it.bz.noi.community-api.csproj -c Release -o /app

FROM build AS test
WORKDIR /src
RUN dotnet test it.bz.noi.community-api.sln

FROM build AS publish
RUN dotnet publish it.bz.noi.community-api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "it.bz.noi.community-api.dll"]
