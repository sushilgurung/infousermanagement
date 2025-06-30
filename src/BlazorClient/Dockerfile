# Stage 1: Build the Blazor WebAssembly app=
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /frontend 

COPY BlazorClient.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /frontend/publish


# Serve the Static file with NGIX
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=build /frontend/publish/wwwroot .
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
