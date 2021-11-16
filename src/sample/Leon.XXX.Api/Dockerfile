#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["sample/Leon.XXX.Api/Leon.XXX.Api.csproj", "sample/Leon.XXX.Api/"]
COPY ["sample/Leon.XXX.Domain/Leon.XXX.Domain.csproj", "sample/Leon.XXX.Domain/"]
COPY ["Library/NetPro.Web.Core/NetPro.Web.Core.csproj", "Library/NetPro.Web.Core/"]
COPY ["Library/NetPro.Core/NetPro.Core.csproj", "Library/NetPro.Core/"]
COPY ["Library/NetPro.TypeFinder/NetPro.TypeFinder.csproj", "Library/NetPro.TypeFinder/"]
COPY ["Library/NetPro.Sign/NetPro.Sign.csproj", "Library/NetPro.Sign/"]
COPY ["Library/NetPro.ShareRequestData/NetPro.ShareRequestBody.csproj", "Library/NetPro.ShareRequestData/"]
COPY ["Library/NetPro.ResponseCache/NetPro.ResponseCache.csproj", "Library/NetPro.ResponseCache/"]
COPY ["Library/NetPro.CsRedis/NetPro.CsRedis.csproj", "Library/NetPro.CsRedis/"]
COPY ["Library/NetPro.Log/NetPro.Log.csproj", "Library/NetPro.Log/"]
COPY ["Library/NetPro.RabbitMQ/NetPro.RabbitMQ.csproj", "Library/NetPro.RabbitMQ/"]
COPY ["sample/Leon.XXX.Repository/Leon.XXX.Repository.csproj", "sample/Leon.XXX.Repository/"]
COPY ["Library/NetPro.Web.Api/NetPro.Web.Api.csproj", "Library/NetPro.Web.Api/"]
COPY ["Library/NetPro.Swagger/NetPro.Swagger.csproj", "Library/NetPro.Swagger/"]
COPY ["sample/Leon.XXX.Proxy/Leon.XXX.Proxy.csproj", "sample/Leon.XXX.Proxy/"]
COPY ["Library/NetPro.Proxy/NetPro.Proxy.csproj", "Library/NetPro.Proxy/"]
COPY ["NuGet.config", "./"]
RUN dotnet restore "sample/Leon.XXX.Api/Leon.XXX.Api.csproj"
COPY . .
WORKDIR "/src/sample/Leon.XXX.Api"
RUN dotnet build "Leon.XXX.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leon.XXX.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leon.XXX.Api.dll"]