# LionFire.Blazor.Components.MudBlazor.Samples.Wasm (Standalone WebAssembly)

This is a **standalone Blazor WebAssembly** sample application that demonstrates the InlineSelector component and other components from the LionFire.Blazor.Components.MudBlazor library.

## What is Standalone Blazor WebAssembly?

This application runs **entirely in the browser** as WebAssembly with **no server required** after the initial download. It can be:
- Deployed as static files to any web host (GitHub Pages, Azure Static Web Apps, Netlify, etc.)
- Hosted on a CDN
- Run offline after initial load

## Features

- Pure client-side execution
- Dark mode support (toggle in top right corner)
- InlineSelector component with:
  - Multiple size options (Large, Medium, Small, Compact, Abbreviated, SingleLetter, Square)
  - Favorites filtering
  - Dropdown view with all items
  - Item creation
  - Custom colors

## Running the Application

### Development Mode

```bash
cd /mnt/c/src/Core/samples/LionFire.Blazor.Components.MudBlazor.Samples.Wasm
dotnet run
```

Or in Windows:
```powershell
cd C:\src\Core\samples\LionFire.Blazor.Components.MudBlazor.Samples.Wasm
dotnet run
```

Then navigate to `https://localhost:7156` (or the HTTP port `http://localhost:5287`).

### Testing Static Deployment Locally

1. **Publish the application:**
   ```bash
   dotnet publish -c Release
   ```

2. **Navigate to the output directory:**
   ```bash
   cd bin/Release/net9.0/publish/wwwroot
   ```

3. **Serve with any static file server:**

   **Using npx http-server (Node.js):**
   ```bash
   npx http-server -p 8080
   ```

   **Using Python:**
   ```bash
   python -m http.server 8080
   ```

   **Using dotnet serve:**
   ```bash
   dotnet tool install -g dotnet-serve
   dotnet serve -p 8080
   ```

4. **Open your browser:**
   Navigate to `http://localhost:8080`

## Deployment Options

### GitHub Pages

1. Publish the app:
   ```bash
   dotnet publish -c Release -o publish
   ```

2. Copy contents of `publish/wwwroot` to your gh-pages branch

3. Add a `.nojekyll` file to prevent GitHub from processing files:
   ```bash
   touch publish/wwwroot/.nojekyll
   ```

### Azure Static Web Apps

```bash
# Using Azure CLI
az staticwebapp create \
  --name my-blazor-app \
  --resource-group my-resource-group \
  --source publish/wwwroot \
  --location "East US 2"
```

### Netlify

1. Publish the app:
   ```bash
   dotnet publish -c Release -o publish
   ```

2. Drag and drop the `publish/wwwroot` folder to Netlify

Or use Netlify CLI:
```bash
netlify deploy --prod --dir=publish/wwwroot
```

### AWS S3 + CloudFront

```bash
# Publish
dotnet publish -c Release -o publish

# Upload to S3
aws s3 sync publish/wwwroot s3://my-bucket-name --delete

# Configure S3 for static website hosting in AWS Console
```

### Docker (with nginx)

Create a `Dockerfile`:
```dockerfile
FROM nginx:alpine
COPY bin/Release/net9.0/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
```

Build and run:
```bash
dotnet publish -c Release
docker build -t blazor-wasm-app .
docker run -p 8080:80 blazor-wasm-app
```

## Important Notes for Deployment

### MIME Types
Ensure your web server serves these MIME types correctly:
- `.wasm` → `application/wasm`
- `.dll` → `application/octet-stream`
- `.json` → `application/json`
- `.blat` → `application/octet-stream`
- `.dat` → `application/octet-stream`

### Compression
For best performance, enable Brotli or gzip compression on your web server. Blazor WASM files are large but compress very well.

### SPA Routing
If using client-side routing, configure your server to return `index.html` for all routes:

**nginx example:**
```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

## Advantages

✅ No server required  
✅ Can run offline  
✅ Deploy as static files  
✅ Scalable (CDN-friendly)  
✅ Full .NET in the browser  

## Limitations

⚠️ Larger initial download size (includes .NET runtime)  
⚠️ No server-side code execution  
⚠️ No real-time SignalR features  
