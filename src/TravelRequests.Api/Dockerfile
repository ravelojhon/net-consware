# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK as build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/TravelRequests.Api/TravelRequests.Api.csproj", "src/TravelRequests.Api/"]
COPY ["src/TravelRequests.Application/TravelRequests.Application.csproj", "src/TravelRequests.Application/"]
COPY ["src/TravelRequests.Domain/TravelRequests.Domain.csproj", "src/TravelRequests.Domain/"]
COPY ["src/TravelRequests.Infrastructure/TravelRequests.Infrastructure.csproj", "src/TravelRequests.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/TravelRequests.Api/TravelRequests.Api.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/src/TravelRequests.Api"
RUN dotnet build "TravelRequests.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "TravelRequests.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "TravelRequests.Api.dll"]