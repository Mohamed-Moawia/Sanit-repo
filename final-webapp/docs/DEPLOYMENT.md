# Deployment Guide - Stationery Store Egypt

## Production Deployment Checklist

### Prerequisites
- [ ] Docker & Docker Compose installed on server
- [ ] SSL certificate configured
- [ ] Domain name configured
- [ ] Database backup strategy in place
- [ ] Monitoring solution configured

### 1. Server Setup

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker --version
docker-compose --version
```

### 2. Clone Repository

```bash
cd /opt
git clone <repository-url> stationery-store
cd stationery-store/final-webapp
```

### 3. Configure Environment

Create `.env` file:

```bash
# Database
POSTGRES_DB=StationeryStore
POSTGRES_USER=stationery_admin
POSTGRES_PASSWORD=<SecurePassword>

# Redis
REDIS_PASSWORD=<SecurePassword>

# JWT
JWT_SECRET=<SuperSecretKeyMin32Characters>
JWT_ISSUER=StationeryStoreEgypt
JWT_AUDIENCE=StationeryStoreUsers

# ETA
ETA_CLIENT_ID=<your-client-id>
ETA_CLIENT_SECRET=<your-client-secret>
```

### 4. Update docker-compose.yaml

Edit production settings:
- Change passwords
- Update JWT secret
- Configure ETA credentials
- Set correct domain names

### 5. Deploy

```bash
# Start services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f stationery-api
```

### 6. Verify Deployment

```bash
# Health check
curl http://localhost:8080/health

# Check API
curl http://localhost:8080/egypt/info

# Test login
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"admin","password":"Admin@123"}'
```

### 7. Configure SSL (Nginx Reverse Proxy)

Create `/etc/nginx/sites-available/stationery-store`:

```nginx
server {
    listen 80;
    server_name your-domain.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name your-domain.com;

    ssl_certificate /etc/letsencrypt/live/your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/your-domain.com/privkey.pem;

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/stationery-store /etc/nginx/sites-enabled/

# Test configuration
sudo nginx -t

# Reload nginx
sudo systemctl reload nginx

# Install SSL certificate
sudo certbot --nginx -d your-domain.com
```

### 8. Backup Strategy

#### Database Backup

```bash
#!/bin/bash
# backup.sh
DATE=$(date +%Y%m%d_%H%M%S)
docker exec stationery-postgres pg_dump -U stationery_admin StationeryStore > /backups/stationery_$DATE.sql

# Keep last 30 days
find /backups -name "stationery_*.sql" -mtime +30 -delete
```

#### Automated Backup (Cron)

```bash
# Edit crontab
crontab -e

# Add daily backup at 2 AM
0 2 * * * /opt/stationery-store/final-webapp/backup.sh
```

### 9. Monitoring

#### Docker Container Monitoring

```bash
# Container stats
docker stats

# Check logs
docker-compose logs -f

# Restart policy
docker-compose ps
```

#### Application Monitoring

- Configure Application Insights or similar
- Set up log aggregation (ELK, Splunk)
- Configure alerts for errors

### 10. Security Hardening

```bash
# Firewall configuration
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw enable

# Fail2ban
sudo apt install fail2ban
sudo systemctl enable fail2ban
```

## Rollback Procedure

```bash
# Stop current deployment
docker-compose down

# Restore database backup
docker exec -i stationery-postgres psql -U stationery_admin -d StationeryStore < /backups/stationery_YYYYMMDD_HHMMSS.sql

# Start previous version
git checkout <previous-tag>
docker-compose up -d
```

## Troubleshooting

### Container Won't Start

```bash
# Check logs
docker-compose logs stationery-api

# Check resource usage
docker stats

# Restart services
docker-compose restart
```

### Database Connection Issues

```bash
# Check database status
docker exec stationery-postgres pg_isready

# Test connection
docker exec -it stationery-postgres psql -U stationery_admin -d StationeryStore
```

### Performance Issues

```bash
# Check container resources
docker stats

# Analyze slow queries
docker exec -it stationery-postgres psql -U stationery_admin -d StationeryStore
# Run: SELECT * FROM pg_stat_statements ORDER BY total_time DESC LIMIT 10;
```

## Contact

For deployment support: devops@stationery.eg
