#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/NuGet.config", "./"]
COPY ["src/sample/Leon.XXX.Api/Leon.XXX.Api.csproj", "src/sample/Leon.XXX.Api/"]
COPY ["src/sample/Leon.XXX.Domain/Leon.XXX.Domain.csproj", "src/sample/Leon.XXX.Domain/"]
COPY ["src/Library/NetPro.Web.Core/NetPro.Web.Core.csproj", "src/Library/NetPro.Web.Core/"]
COPY ["src/Library/NetPro.Core/NetPro.Core.csproj", "src/Library/NetPro.Core/"]
COPY ["src/Library/NetPro.TypeFinder/NetPro.TypeFinder.csproj", "src/Library/NetPro.TypeFinder/"]
COPY ["src/Library/NetPro.Checker/NetPro.Checker.csproj", "src/Library/NetPro.Checker/"]
COPY ["src/Library/NetPro.Sign/NetPro.Sign.csproj", "src/Library/NetPro.Sign/"]
COPY ["src/Library/NetPro.ShareRequestData/NetPro.ShareRequestBody.csproj", "src/Library/NetPro.ShareRequestData/"]
COPY ["src/Library/NetPro.ResponseCache/NetPro.ResponseCache.csproj", "src/Library/NetPro.ResponseCache/"]
COPY ["src/Library/NetPro.CsRedis/NetPro.CsRedis.csproj", "src/Library/NetPro.CsRedis/"]
COPY ["src/Library/NetPro.Log/NetPro.Log.csproj", "src/Library/NetPro.Log/"]
COPY ["src/Library/NetPro.RabbitMQ/NetPro.RabbitMQ.csproj", "src/Library/NetPro.RabbitMQ/"]
COPY ["src/sample/Leon.XXX.Repository/Leon.XXX.Repository.csproj", "src/sample/Leon.XXX.Repository/"]
COPY ["src/Library/NetPro.Web.Api/NetPro.Web.Api.csproj", "src/Library/NetPro.Web.Api/"]
COPY ["src/Library/NetPro.Swagger/NetPro.Swagger.csproj", "src/Library/NetPro.Swagger/"]
COPY ["src/sample/Leon.XXX.Proxy/Leon.XXX.Proxy.csproj", "src/sample/Leon.XXX.Proxy/"]
COPY ["src/Library/NetPro.Proxy/NetPro.Proxy.csproj", "src/Library/NetPro.Proxy/"]
COPY ["src/Directory.Build.props", "./"]
COPY ["docs", "./"]
RUN dotnet restore "src/sample/Leon.XXX.Api/Leon.XXX.Api.csproj"
COPY . .
WORKDIR "/src/src/sample/Leon.XXX.Api"
RUN dotnet build "Leon.XXX.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leon.XXX.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leon.XXX.Api.dll"]