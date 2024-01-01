$services = @("NotificationService1", "NotificationService2", "GrpcOrderService", "ProductService")

foreach ($service in $services) {
    $servicePath = Join-Path (Get-Location) MiddlewareAssignment\$service
    $csprojPath = Join-Path $servicePath "$service.csproj"

    if (Test-Path $csprojPath) {
        Write-Host "Restoring $service..."
        dotnet restore $csprojPath

        Write-Host "Building $service..."
        dotnet build --configuration Release $csprojPath

        Write-Host "Running $service..."
        Start-Process powershell -ArgumentList "dotnet run $csprojPath" -WorkingDirectory $servicePath
        Start-Sleep -Seconds 10
    } else {
        Write-Host "Error: $csprojPath not found."
    }
}

Write-Host "All services started."
