dotnet nuget push ./PushNuget/*.nupkg -k key -s https://www.nuget.org/api/v2/package
Write-Output "ok"