FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/BlazorClient/BlazorClient.csproj ./BlazorClient/
RUN dotnet restore ./BlazorClient/BlazorClient.csproj

COPY src/BlazorClient/ ./BlazorClient/
WORKDIR /src/BlazorClient

RUN dotnet publish BlazorClient.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS runtime
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
