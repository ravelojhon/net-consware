# Use the official .NET 9 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/TravelRequests.Api/TravelRequests.Api.csproj", "src/TravelRequests.Api/"]
COPY ["src/TravelRequests.Application/TravelRequests.Application.csproj", "src/TravelRequests.Application/"]
COPY ["src/TravelRequests.Domain/TravelRequests.Domain.csproj", "src/TravelRequests.Domain/"]
COPY ["src/TravelRequests.Infrastructure/TravelRequests.Infrastructure.csproj", "src/TravelRequests.Infrastructure/"]

RUN dotnet restore "src/TravelRequests.Api/TravelRequests.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/TravelRequests.Api"
RUN dotnet build "TravelRequests.Api.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "TravelRequests.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelRequests.Api.dll"]
