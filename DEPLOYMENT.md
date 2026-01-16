# Deployment Guide for Wager Wise Journal

This guide covers deployment options for the Wager Wise Journal application.

## Table of Contents
1. [Development Environment](#development-environment)
2. [Production Deployment Options](#production-deployment-options)
3. [Azure App Service Deployment](#azure-app-service-deployment)
4. [Docker Deployment](#docker-deployment)
5. [IIS Deployment](#iis-deployment)
6. [Linux Server Deployment](#linux-server-deployment)

---

## Development Environment

### Running Locally

1. **Configure the database** (see DATABASE_SETUP.md)

2. **Set up OAuth providers** (optional for local testing):
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Authentication:Google:ClientId" "your-client-id"
   dotnet user-secrets set "Authentication:Google:ClientSecret" "your-client-secret"
   dotnet user-secrets set "Authentication:Facebook:AppId" "your-app-id"
   dotnet user-secrets set "Authentication:Facebook:AppSecret" "your-app-secret"
   dotnet user-secrets set "GoogleMaps:ApiKey" "your-maps-api-key"
   ```

3. **Run the application:**
   ```bash
   cd WagerWiseJournal
   dotnet run
   ```

4. **Access the application:**
   - HTTPS: https://localhost:5001
   - HTTP: http://localhost:5000

---

## Production Deployment Options

### Prerequisites for All Deployments

1. **Configure environment-specific settings:**
   - Database connection strings
   - OAuth client credentials
   - Google Maps API key
   - HTTPS certificates

2. **Build the application:**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

---

## Azure App Service Deployment

### Step 1: Create Azure Resources

```bash
# Install Azure CLI if needed
# https://docs.microsoft.com/cli/azure/install-azure-cli

# Login to Azure
az login

# Create resource group
az group create --name wwj-rg --location eastus

# Create App Service plan
az appservice plan create --name wwj-plan --resource-group wwj-rg --sku B1 --is-linux

# Create web app
az webapp create --resource-group wwj-rg --plan wwj-plan --name your-unique-app-name --runtime "DOTNETCORE:8.0"

# Create Azure Database for MySQL
az mysql flexible-server create --resource-group wwj-rg --name wwj-mysql-server --admin-user wwjadmin --admin-password YourSecurePassword123! --sku-name Standard_B1ms --tier Burstable --version 8.0.21
```

### Step 2: Configure Application Settings

```bash
# Set connection string
az webapp config connection-string set --resource-group wwj-rg --name your-unique-app-name --settings DefaultConnection="Server=wwj-mysql-server.mysql.database.azure.com;Database=WagerWiseJournal;User=wwjadmin;Password=YourSecurePassword123!;SslMode=Required;" --connection-string-type MySql

# Set OAuth settings
az webapp config appsettings set --resource-group wwj-rg --name your-unique-app-name --settings \
  Authentication__Google__ClientId="your-google-client-id" \
  Authentication__Google__ClientSecret="your-google-secret" \
  Authentication__Facebook__AppId="your-facebook-app-id" \
  Authentication__Facebook__AppSecret="your-facebook-secret" \
  GoogleMaps__ApiKey="your-google-maps-key"
```

### Step 3: Deploy the Application

```bash
# Using Azure CLI
az webapp up --resource-group wwj-rg --name your-unique-app-name --runtime "DOTNETCORE:8.0"

# Or using zip deploy
cd publish
zip -r ../app.zip .
az webapp deployment source config-zip --resource-group wwj-rg --name your-unique-app-name --src ../app.zip
```

### Step 4: Run Database Migrations

```bash
# Update connection string locally to point to Azure MySQL
# Then run migrations
dotnet ef database update
```

---

## Docker Deployment

### Step 1: Create Dockerfile

Create `Dockerfile` in the solution root:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WagerWiseJournal/WagerWiseJournal.csproj", "WagerWiseJournal/"]
RUN dotnet restore "WagerWiseJournal/WagerWiseJournal.csproj"
COPY . .
WORKDIR "/src/WagerWiseJournal"
RUN dotnet build "WagerWiseJournal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WagerWiseJournal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WagerWiseJournal.dll"]
```

### Step 2: Create docker-compose.yml

```yaml
version: '3.8'
services:
  web:
    build: .
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=mysql;Database=WagerWiseJournal;User=root;Password=SecurePassword123!;
      - Authentication__Google__ClientId=${GOOGLE_CLIENT_ID}
      - Authentication__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
      - Authentication__Facebook__AppId=${FACEBOOK_APP_ID}
      - Authentication__Facebook__AppSecret=${FACEBOOK_APP_SECRET}
      - GoogleMaps__ApiKey=${GOOGLE_MAPS_API_KEY}
    depends_on:
      - mysql
  
  mysql:
    image: mysql:8.0
    environment:
      - MYSQL_ROOT_PASSWORD=SecurePassword123!
      - MYSQL_DATABASE=WagerWiseJournal
    ports:
      - "3306:3306"
    volumes:
      - mysql-data:/var/lib/mysql

volumes:
  mysql-data:
```

### Step 3: Deploy with Docker

```bash
# Build and run
docker-compose up -d

# Run migrations
docker-compose exec web dotnet ef database update

# View logs
docker-compose logs -f web
```

---

## IIS Deployment (Windows Server)

### Prerequisites
- Windows Server with IIS installed
- .NET 8.0 Hosting Bundle installed
- MySQL Server installed or accessible

### Step 1: Publish the Application

```bash
dotnet publish -c Release -o C:\inetpub\wwwroot\WagerWiseJournal
```

### Step 2: Create IIS Application Pool

1. Open IIS Manager
2. Create new Application Pool named "WagerWiseJournal"
3. Set .NET CLR version to "No Managed Code"
4. Set Managed pipeline mode to "Integrated"

### Step 3: Create IIS Website

1. In IIS Manager, create new website
2. Set physical path to `C:\inetpub\wwwroot\WagerWiseJournal`
3. Assign the Application Pool created above
4. Set binding to port 80 or 443 (with SSL certificate)

### Step 4: Configure Environment Variables

In Application Pool > Advanced Settings > Environment Variables, add:
- `ASPNETCORE_ENVIRONMENT`: `Production`
- Connection strings and OAuth settings

### Step 5: Set Permissions

Grant IIS_IUSRS read/execute permissions on the application folder.

---

## Linux Server Deployment (Ubuntu/Debian)

### Step 1: Install Prerequisites

```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 8.0 runtime
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0 --runtime aspnetcore

# Install MySQL
sudo apt install mysql-server -y

# Install Nginx
sudo apt install nginx -y
```

### Step 2: Deploy Application

```bash
# Copy published files to server
scp -r ./publish/* user@server:/var/www/wagerwisejournal/

# Set up systemd service
sudo nano /etc/systemd/system/wagerwisejournal.service
```

Add the following content:

```ini
[Unit]
Description=Wager Wise Journal Application

[Service]
WorkingDirectory=/var/www/wagerwisejournal
ExecStart=/home/user/.dotnet/dotnet /var/www/wagerwisejournal/WagerWiseJournal.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=wagerwisejournal
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false
Environment="ConnectionStrings__DefaultConnection=Server=localhost;Database=WagerWiseJournal;User=wwjuser;Password=password;"

[Install]
WantedBy=multi-user.target
```

### Step 3: Configure Nginx

```bash
sudo nano /etc/nginx/sites-available/wagerwisejournal
```

Add:

```nginx
server {
    listen 80;
    server_name your-domain.com;
    location / {
        proxy_pass http://localhost:5000;
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

### Step 4: Enable and Start Services

```bash
# Enable site
sudo ln -s /etc/nginx/sites-available/wagerwisejournal /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx

# Start application service
sudo systemctl enable wagerwisejournal.service
sudo systemctl start wagerwisejournal.service
sudo systemctl status wagerwisejournal.service
```

### Step 5: Configure SSL (Optional but Recommended)

```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx -y

# Obtain certificate
sudo certbot --nginx -d your-domain.com
```

---

## Post-Deployment Checklist

- [ ] Database migrations completed
- [ ] OAuth redirect URIs updated to production domain
- [ ] Google Maps API key configured with production domain restrictions
- [ ] SSL certificate installed and HTTPS enforced
- [ ] Environment variables/secrets properly configured
- [ ] Application pool/service is running
- [ ] Database backups configured
- [ ] Monitoring and logging enabled
- [ ] Test authentication with all providers
- [ ] Test core functionality (create session, add bets, etc.)

---

## Monitoring and Maintenance

### Application Logs

- **Azure:** Use Application Insights or App Service logs
- **Docker:** `docker-compose logs -f web`
- **IIS:** Check Event Viewer and application logs
- **Linux:** `sudo journalctl -u wagerwisejournal.service -f`

### Database Backups

Schedule regular MySQL backups:

```bash
# Automated backup script
#!/bin/bash
BACKUP_DIR="/backups/mysql"
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
mysqldump -u wwjuser -p'password' WagerWiseJournal > $BACKUP_DIR/wwj_$TIMESTAMP.sql
```

### Health Checks

Monitor application health:
- Check `/` endpoint returns 200
- Verify database connectivity
- Monitor CPU and memory usage
- Set up alerts for errors

---

## Troubleshooting

### Common Issues

1. **Database Connection Errors**
   - Verify connection string format
   - Check firewall rules
   - Ensure MySQL service is running

2. **OAuth Redirect Issues**
   - Update authorized redirect URIs in provider consoles
   - Verify domain matches exactly

3. **Permission Errors (Linux)**
   - Check file permissions: `sudo chown -R www-data:www-data /var/www/wagerwisejournal`

4. **Application Won't Start**
   - Check logs for detailed error messages
   - Verify .NET runtime is installed
   - Check port availability

---

## Support

For issues and questions:
- GitHub Issues: https://github.com/coasterdudebob/wwj/issues
- Documentation: Check README.md and DATABASE_SETUP.md
