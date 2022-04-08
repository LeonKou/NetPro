#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80
WORKDIR /app
COPY . .
ENTRYPOINT ["dotnet", "XXX.GrpcServer.dll","--urls=http://0.0.0.0:80"]