# Publish the Blazor WASM app
Write-Host "Publishing Blazor WASM app..." -ForegroundColor Cyan
dotnet publish -c Release

Write-Host ""
Write-Host "Build complete! Published to: bin\Release\net9.0\publish\wwwroot" -ForegroundColor Green
Write-Host ""
Write-Host "To test the static deployment, run one of these commands:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Option 1 - Using npx http-server:" -ForegroundColor White
Write-Host "  cd bin\Release\net9.0\publish\wwwroot"
Write-Host "  npx http-server -p 8080"
Write-Host ""
Write-Host "Option 2 - Using Python:" -ForegroundColor White
Write-Host "  cd bin\Release\net9.0\publish\wwwroot"
Write-Host "  python -m http.server 8080"
Write-Host ""
Write-Host "Option 3 - Using dotnet serve (recommended for SPA routing):" -ForegroundColor White
Write-Host "  dotnet tool install -g dotnet-serve"
Write-Host "  cd bin\Release\net9.0\publish\wwwroot"
Write-Host "  dotnet serve -p 8080 --fallback-file index.html"
Write-Host ""
Write-Host "Then open http://localhost:8080 in your browser" -ForegroundColor Cyan
Write-Host ""
Write-Host "Note: The --fallback-file flag enables SPA routing so URLs like /inline-selector work correctly" -ForegroundColor Yellow
