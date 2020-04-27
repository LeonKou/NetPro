#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Leon.XXX.Api/Leon.XXX.Api.csproj", "Leon.XXX.Api/"]
COPY ["Leon.Web.Api/Leon.Web.Api.csproj", "Leon.Web.Api/"]
COPY ["Leon.Web.Core/Leon.Web.Core.csproj", "Leon.Web.Core/"]
COPY ["Leon.Core/Leon.Core.csproj", "Leon.Core/"]
COPY ["Leon.Utility/Leon.Utility.csproj", "Leon.Utility/"]
COPY ["Leon.Dapper/Leon.Dapper.csproj", "Leon.Dapper/"]
COPY ["Leon.Checker/Leon.Checker.csproj", "Leon.Checker/"]
COPY ["Leon.MongoDb/Leon.MongoDb.csproj", "Leon.MongoDb/"]
COPY ["Leon.Log/Leon.Log.csproj", "Leon.Log/"]
COPY ["Leon.Cache/Leon.Cache.csproj", "Leon.Cache/"]
COPY ["Leon.XXX.Aggregate/Leon.XXX.Aggregate.csproj", "Leon.XXX.Aggregate/"]
COPY ["Leon.XXX.Domain/Leon.XXX.Domain.csproj", "Leon.XXX.Domain/"]
COPY ["Leon.XXX.Repository/Leon.XXX.Repository.csproj", "Leon.XXX.Repository/"]
COPY ["Leon.XXX.Proxy/Leon.XXX.Proxy.csproj", "Leon.XXX.Proxy/"]
COPY ["NuGet.config", "./"]
RUN dotnet restore "Leon.XXX.Api/Leon.XXX.Api.csproj"
COPY . .
WORKDIR "/src/Leon.XXX.Api"
RUN dotnet build "Leon.XXX.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leon.XXX.Api.csproj" -c Release -o /app/publish

FROM base AS final

ENV ASPNETCORE_ENVIRONMENT #Development/Staging/Production
ENV TZ Asia/Shanghai
ENV ASPNETCORE_HOSTINGSTARTUPASSEMBLIES SkyAPM.Agent.AspNetCore

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leon.XXX.Api.dll"]