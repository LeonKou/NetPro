dotnet nuget push ./PushNuget/*.nupkg -k key -s https://www.nuget.org/api/v2/package  --skip-duplicate
Write-Output "ok"