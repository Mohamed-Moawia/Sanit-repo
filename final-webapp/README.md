# Stationery Store Management System - Egypt

A comprehensive .NET 10 ASP.NET Core Web API for managing stationery stores in Egypt with full Egyptian Tax Authority (ETA) e-invoicing compliance.

## 🌟 Features

### Core Business Modules
- **Product Catalog** - Complete inventory management with Egyptian barcode support
- **Customer Management** - B2B/B2C customers with tax information
- **Sales & POS** - Point of sale with real-time stock management
- **Inventory Management** - Multi-branch stock tracking
- **Supplier & Purchase Orders** - Procurement management
- **ETA E-Invoicing** - Full integration with Egyptian Tax Authority

### Egyptian Compliance
- ✅ VAT (14% standard rate) calculation
- ✅ ETA e-Invoicing API integration
- ✅ Arabic/English bilingual support
- ✅ Egyptian currency formatting (ج.م, 3 decimals)
- ✅ Tax registration number validation
- ✅ QR code generation for receipts

### Technical Features
- ✅ .NET 10 with latest best practices
- ✅ Clean Architecture (Domain-Driven Design)
- ✅ Repository Pattern with Specifications
- ✅ JWT Authentication & Authorization
- ✅ Redis Caching
- ✅ PostgreSQL Database
- ✅ Serilog Logging
- ✅ Health Checks
- ✅ Docker & Docker Compose
- ✅ Swagger/OpenAPI Documentation

## 🏗 Architecture

```
final-webapp/
├── src/
│   ├── StationeryStore.Domain/        # Domain entities, enums, value objects
│   ├── StationeryStore.Application/   # DTOs, interfaces, services
│   ├── StationeryStore.Infrastructure/# DbContext, repositories, external services
│   └── StationeryStore.API/           # Controllers, middleware, configuration
├── tests/
│   └── StationeryStore.Tests/         # Unit & integration tests
├── docker/
│   └── init.sql                       # Database initialization
├── docker-compose.yaml                # Production configuration
├── docker-compose.dev.yaml            # Development configuration
└── Dockerfile                         # Multi-stage build
```

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 10 SDK (for local development)

### Running with Docker (Recommended)

```bash
# Navigate to project root
cd final-webapp

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f stationery-api

# Access Swagger UI
open http://localhost:8080/api-docs

# Health check
curl http://localhost:8080/health
```

### Local Development

```bash
# Start infrastructure only (PostgreSQL, Redis)
docker-compose -f docker-compose.dev.yaml up -d

# Run the API locally
cd src/StationeryStore.API
dotnet run

# Access Swagger UI
open http://localhost:5000/api-docs
```

## 📝 API Documentation

Once running, access the interactive API documentation:
- **Swagger UI:** http://localhost:8080/api-docs
- **Health Check:** http://localhost:8080/health
- **Egypt Info:** http://localhost:8080/egypt/info

### Authentication

All endpoints (except `/auth/login`) require JWT Bearer token:

```bash
# Login to get token
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "usernameOrEmail": "admin",
    "password": "Admin@123"
  }'

# Use token in subsequent requests
curl -X GET http://localhost:8080/api/products \
  -H "Authorization: Bearer {your_token}"
```

### Default Credentials

After first run:
- **Username:** admin
- **Password:** Admin@123

**⚠️ Change default credentials in production!**

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | `Production` |
| `ConnectionStrings__PostgreSQL` | Database connection | - |
| `ConnectionStrings__Redis` | Redis connection | - |
| `JwtSettings__Secret` | JWT signing key | - |
| `EgyptSettings__DefaultVatRate` | VAT rate | `14.0` |

### appsettings.json

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Port=5432;Database=StationeryStore;Username=stationery_admin;Password=YourSecurePassword",
    "Redis": "localhost:6379,password=YourRedisPassword"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyMin32CharactersLong",
    "Issuer": "StationeryStoreEgypt",
    "Audience": "StationeryStoreUsers",
    "ExpiryInMinutes": 480
  },
  "EgyptSettings": {
    "DefaultVatRate": 14.0,
    "TimeZone": "Africa/Cairo",
    "DefaultLanguage": "ar"
  }
}
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test category
dotnet test --filter "Category=Unit"
```

## 📦 Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/StationeryStore.Infrastructure --startup-project src/StationeryStore.API

# Update database
dotnet ef database update --project src/StationeryStore.Infrastructure --startup-project src/StationeryStore.API

# Remove migration
dotnet ef migrations remove --project src/StationeryStore.Infrastructure --startup-project src/StationeryStore.API
```

## 🔐 Security Considerations

1. **Change Default Secrets:** Update all passwords and JWT secrets in production
2. **Enable HTTPS:** Configure SSL certificates for production
3. **Use Environment Variables:** Store secrets in environment variables or Azure Key Vault
4. **Rate Limiting:** Implement rate limiting for API endpoints
5. **CORS:** Configure allowed origins for production domains

## 📊 Monitoring

### Health Checks

- `/health` - Overall health status
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

### Logging

Logs are written to:
- Console (stdout)
- File: `logs/log-.txt` (daily rolling, 30 days retention)

## 🌍 Localization

The API supports Arabic and English:

```bash
# Arabic (default)
curl http://localhost:8080/api/products

# English
curl -H "Accept-Language: en" http://localhost:8080/api/products
```

## 📄 License

Commercial License - ترخيص تجاري
© 2026 Stationery Store Egypt. All rights reserved.

## 🤝 Support

For technical support:
- **Email:** support@stationery.eg
- **Documentation:** http://localhost:8080/api-docs

## 🏛 Egyptian Tax Authority (ETA) Integration

### Configuration

Update `EtaSettings` in appsettings.json:

```json
{
  "EtaSettings": {
    "BaseUrl": "https://api.preprod.invoicing.eta.gov.eg",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "Environment": "Sandbox"
  }
}
```

### Invoice Submission

Invoices are automatically submitted to ETA upon creation. Check submission status:

```bash
curl -X GET http://localhost:8080/api/eta/submissions/{submissionId}/status \
  -H "Authorization: Bearer {token}"
```

---

**Built with .NET 10 • ASP.NET Core • PostgreSQL • Redis**
