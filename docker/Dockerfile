# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["/var/home/mo/SanitaryCeramicCMS/src/Web/Web.csproj", "src/Web/"]
COPY ["/var/home/mo/SanitaryCeramicCMS/src/Application/Application.csproj", "src/Application/"]
COPY ["/var/home/mo/SanitaryCeramicCMS/src/Domain/Domain.csproj", "src/Domain/"]
COPY ["/var/home/mo/SanitaryCeramicCMS/src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Web/Web.csproj"

COPY . .
WORKDIR "/src/src/Web"
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SanitaryCeramicCMS.Web.dll"]