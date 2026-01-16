# Database Setup Script for Wager Wise Journal

This document provides instructions for setting up the MySQL database.

## Prerequisites

- MySQL Server 8.0 or later installed and running
- .NET 8.0 SDK installed
- EF Core tools installed (run `dotnet tool install --global dotnet-ef` if not already installed)

## Setup Steps

### 1. Create MySQL Database

Connect to your MySQL server and create the database:

```sql
CREATE DATABASE WagerWiseJournal;
```

### 2. Create MySQL User (Optional but recommended)

```sql
CREATE USER 'wwjuser'@'localhost' IDENTIFIED BY 'YourSecurePassword123!';
GRANT ALL PRIVILEGES ON WagerWiseJournal.* TO 'wwjuser'@'localhost';
FLUSH PRIVILEGES;
```

### 3. Update Connection String

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WagerWiseJournal;User=wwjuser;Password=YourSecurePassword123!;"
  }
}
```

**For Production:** Use environment variables or Azure Key Vault:

```bash
export ConnectionStrings__DefaultConnection="Server=your-server;Database=WagerWiseJournal;User=your-user;Password=your-password;"
```

### 4. Run Database Migrations

From the project directory:

```bash
cd WagerWiseJournal
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This will create all necessary tables:
- AspNetUsers (and other Identity tables)
- Casinos
- BettingSessions
- Bets

### 5. Verify Database Schema

Connect to MySQL and verify tables were created:

```sql
USE WagerWiseJournal;
SHOW TABLES;
```

You should see:
- AspNetRoles
- AspNetRoleClaims
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserRoles
- AspNetUsers
- AspNetUserTokens
- Bets
- BettingSessions
- Casinos
- __EFMigrationsHistory

## Optional: Seed Sample Data

You can add sample casinos for testing:

```sql
USE WagerWiseJournal;

INSERT INTO Casinos (Name, Address, City, State, ZipCode, Country, Latitude, Longitude, CreatedDate)
VALUES 
('Bellagio Hotel and Casino', '3600 S Las Vegas Blvd', 'Las Vegas', 'NV', '89109', 'USA', 36.1129, -115.1765, UTC_TIMESTAMP()),
('MGM Grand', '3799 S Las Vegas Blvd', 'Las Vegas', 'NV', '89109', 'USA', 36.1024, -115.1699, UTC_TIMESTAMP()),
('Caesars Palace', '3570 S Las Vegas Blvd', 'Las Vegas', 'NV', '89109', 'USA', 36.1162, -115.1745, UTC_TIMESTAMP()),
('The Venetian', '3355 S Las Vegas Blvd', 'Las Vegas', 'NV', '89109', 'USA', 36.1212, -115.1697, UTC_TIMESTAMP()),
('Wynn Las Vegas', '3131 S Las Vegas Blvd', 'Las Vegas', 'NV', '89109', 'USA', 36.1274, -115.1653, UTC_TIMESTAMP());
```

## Troubleshooting

### Connection Issues

If you encounter connection errors:

1. **Check MySQL is running:**
   ```bash
   sudo systemctl status mysql  # Linux
   brew services list           # macOS with Homebrew
   ```

2. **Verify connection string format:**
   - Ensure server name is correct (localhost, IP address, or hostname)
   - Verify port if not default (3306)
   - Check username and password

3. **Test connection manually:**
   ```bash
   mysql -h localhost -u wwjuser -p WagerWiseJournal
   ```

### Migration Errors

If migrations fail:

1. **Remove previous migration:**
   ```bash
   dotnet ef migrations remove
   ```

2. **Clean and rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```

3. **Re-create migration:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### Devart MySQL Connector Issues

If you prefer to use only Devart connector:

1. The project already includes both Devart.Data.MySql and Pomelo.EntityFrameworkCore.MySql
2. Pomelo is used by EF Core for migrations
3. Devart can be used for direct database access if needed

## Database Backup

To backup your database:

```bash
mysqldump -u wwjuser -p WagerWiseJournal > wwj_backup_$(date +%Y%m%d).sql
```

To restore:

```bash
mysql -u wwjuser -p WagerWiseJournal < wwj_backup_20260116.sql
```

## Security Notes

- **Never commit** database credentials to source control
- Use **environment variables** or **secret managers** for sensitive data
- Implement **regular backups** in production
- Use **SSL/TLS** connections in production environments
- Follow the **principle of least privilege** for database users
