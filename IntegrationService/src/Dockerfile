# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ./IntegrationSimulator.IntegrationService.Program/IntegrationSimulator.IntegrationService.Program.csproj ./IntegrationSimulator.IntegrationService.Program/
RUN dotnet restore ./IntegrationSimulator.IntegrationService.Program/IntegrationSimulator.IntegrationService.Program.csproj

# Copy everything else and build
COPY ./IntegrationSimulator.IntegrationService.Program/. ./IntegrationSimulator.IntegrationService.Program/
WORKDIR /src/IntegrationSimulator.IntegrationService.Program
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

# Define entrypoint
ENTRYPOINT ["dotnet", "IntegrationSimulator.IntegrationService.Program.dll"]
