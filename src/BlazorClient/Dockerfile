# Stage 1: Build the Blazor WebAssembly app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy project files
COPY . . 
# Restore dependencies (optional, improves build caching)
RUN dotnet restore
# Publish the Blazor WASM app
RUN dotnet publish -c Release -o /app/out

COPY wwwroot/appsettings.Docker.json /app/out/wwwroot/appsettings.json

# Stage 2: Serve with NGINX
FROM nginx:alpine AS final

# Copy NGINX configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Copy published files to NGINX html folder
COPY --from=build /app/out/wwwroot /usr/share/nginx/html

EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]
