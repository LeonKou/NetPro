#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#Dockerfile和应用强关联，不建议单独文件夹统一管理；
dotnet build ;dotnet publish ;docker build 等命令都可在此目录下执行。
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "XXX.API.dll"]