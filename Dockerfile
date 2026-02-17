# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Scaffold a new Web API project (generates .csproj)
RUN dotnet new webapi -n YourWebApiApp

# Copy your existing source files into the project folder
COPY . ./YourWebApiApp

WORKDIR /src/YourWebApiApp

# Restore dependencies
RUN dotnet restore

# Publish the application in Release mode
RUN dotnet publish -c Release -o /app/publish


# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Create non-root user for security (Podman-friendly)
RUN adduser --disabled-password --gecos "" appuser \
    && chown -R appuser /app
USER appuser

# Expose default ASP.NET Core port
EXPOSE 8080

# Persistent volume for logs, configs, and audit evidence
VOLUME ["/app/data"]

# Healthcheck for container monitoring
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Environment variables (override at runtime with `podman run -e`)
ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_gcServer=1 \
    ASPNETCORE_URLS=http://+:8080 \
    AUDIT_LOG_PATH=/app/data/audit.log \
    APP_LOG_PATH=/app/data/app.log

# CIS audit evidence hook: log startup configs
ENTRYPOINT ["/bin/sh", "-c", "echo 'Container started at $(date)' >> $AUDIT_LOG_PATH && \
  echo 'Environment: DOTNET_RUNNING_IN_CONTAINER=$DOTNET_RUNNING_IN_CONTAINER, ASPNETCORE_URLS=$ASPNETCORE_URLS' >> $AUDIT_LOG_PATH && \
  dotnet YourWebApiApp.dll >> $APP_LOG_PATH 2>&1"]

