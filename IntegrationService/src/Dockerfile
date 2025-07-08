# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all source code 
COPY . .

# Restore and publish the main project
WORKDIR /src/IntegrationSimulator.IntegrationService.Program
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "IntegrationSimulator.IntegrationService.Program.dll"]
