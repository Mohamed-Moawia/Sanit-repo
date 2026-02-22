# ==============================================================================
# STAGE 1: BUILD
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution file
COPY StationeryStore.sln ./

# Copy all project files first for better layer caching
COPY src/StationeryStore.API/StationeryStore.API.csproj ./src/StationeryStore.API/
COPY src/StationeryStore.Domain/StationeryStore.Domain.csproj ./src/StationeryStore.Domain/
COPY src/StationeryStore.Application/StationeryStore.Application.csproj ./src/StationeryStore.Application/
COPY src/StationeryStore.Infrastructure/StationeryStore.Infrastructure.csproj ./src/StationeryStore.Infrastructure/
COPY tests/StationeryStore.Tests/StationeryStore.Tests.csproj ./tests/StationeryStore.Tests/

# Restore dependencies (cached layer)
RUN dotnet restore

# Copy all source code
COPY . .

# Build all projects
RUN dotnet build StationeryStore.sln -c Release --no-restore

# Publish the API project
RUN dotnet publish src/StationeryStore.API/StationeryStore.API.csproj -c Release -o /app/publish --no-build /p:UseAppHost=false

# ==============================================================================
# STAGE 2: RUNTIME
# ==============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Install curl for health checks and create non-root user
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && (getent passwd appuser >/dev/null 2>&1 || useradd -r -u 1001 -g root appuser)

# Copy published output from build stage
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:root /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Configure environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_gcServer=1
ENV TZ=Africa/Cairo

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "StationeryStore.API.dll"]
