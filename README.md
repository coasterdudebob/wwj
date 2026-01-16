# Wager Wise Journal - C# MVC Cross-Platform Casino Bet Tracking Application

A comprehensive ASP.NET Core MVC application for tracking casino betting sessions across multiple locations. This application provides cross-platform support (accessible via web browsers on iOS, Android, and desktop), MySQL database integration using Devart connector, Google Maps integration for casino location identification, and OAuth authentication via Microsoft Identity with Google, Apple, and Facebook providers.

## Features

### Authentication & Security
- **Microsoft Identity Integration**: Complete user management with ASP.NET Core Identity
- **OAuth Providers**: Support for Google, Apple, and Facebook authentication
- **Secure Password Management**: Industry-standard password hashing and validation
- **Role-based Authorization**: Secure access control for different user types

### Casino Location Management
- **Google Maps Integration**: Visual location-based identification of casinos
- **Geolocation Support**: Find nearby casinos based on user's current location
- **Comprehensive Casino Database**: Store and manage casino information including:
  - Name, address, city, state, zip code, country
  - GPS coordinates (latitude/longitude)
  - Automatic distance calculation

### Betting Session Tracking
- **Session Management**: Track betting sessions on a per-location basis
- **Real-time Session Status**: Active session monitoring
- **Session History**: Complete historical record of all betting sessions
- **Per-Session Statistics**: 
  - Total wagered amount
  - Total winnings
  - Net profit/loss
  - Session duration

### Bet Recording
- **Individual Bet Tracking**: Record each bet made during a session
- **Game Type Classification**: Track different types of casino games
- **Detailed Bet Information**:
  - Bet amount
  - Winnings
  - Timestamp
  - Game description
  - Net result calculation

### Cross-Platform Support
- **Responsive Web Design**: Accessible from any device with a web browser
- **iOS Compatibility**: Full support via Safari and other iOS browsers
- **Android Compatibility**: Full support via Chrome and other Android browsers
- **Desktop Support**: Works on Windows, macOS, and Linux

## Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: MySQL with Devart.Data.MySql connector
- **ORM**: Entity Framework Core 8.0 with Pomelo.EntityFrameworkCore.MySql
- **Authentication**: Microsoft Identity with OAuth 2.0
- **Maps**: Google Maps JavaScript API (ready for integration)
- **UI**: Bootstrap 5 (responsive design)
- **Language**: C# 12

## Prerequisites

- .NET 8.0 SDK or later
- MySQL Server 8.0 or later
- Google Cloud Platform account (for Google Maps API and OAuth)
- Facebook Developer account (for Facebook OAuth)
- Apple Developer account (for Apple OAuth)

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/coasterdudebob/wwj.git
cd wwj
```

### 2. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=WagerWiseJournal;User=your_user;Password=your_password;"
  }
}
```

### 3. Configure OAuth Providers

#### Google OAuth Setup
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URIs: `https://yourdomain.com/signin-google`
6. Update `appsettings.json`:

```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your_google_client_id",
      "ClientSecret": "your_google_client_secret"
    }
  }
}
```

#### Facebook OAuth Setup
1. Go to [Facebook Developers](https://developers.facebook.com/)
2. Create a new app
3. Add Facebook Login product
4. Configure OAuth redirect URIs: `https://yourdomain.com/signin-facebook`
5. Update `appsettings.json`:

```json
{
  "Authentication": {
    "Facebook": {
      "AppId": "your_facebook_app_id",
      "AppSecret": "your_facebook_app_secret"
    }
  }
}
```

#### Google Maps Setup
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Enable Maps JavaScript API
3. Create an API key
4. Restrict the API key to your domain
5. Update `appsettings.json`:

```json
{
  "GoogleMaps": {
    "ApiKey": "your_google_maps_api_key"
  }
}
```

### 4. Database Migration

```bash
cd WagerWiseJournal
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Build and Run

```bash
dotnet build
dotnet run
```

The application will be available at `https://localhost:5001` (or the port specified in your configuration).

## Project Structure

```
wwj/
├── WagerWiseJournal/
│   ├── Controllers/
│   │   ├── AccountController.cs      # Authentication and user management
│   │   ├── BetController.cs          # Bet CRUD operations
│   │   ├── CasinoController.cs       # Casino management and location services
│   │   ├── SessionController.cs      # Betting session management
│   │   └── HomeController.cs         # Home and dashboard
│   ├── Data/
│   │   └── ApplicationDbContext.cs   # EF Core database context
│   ├── Models/
│   │   ├── ApplicationUser.cs        # User model extending IdentityUser
│   │   ├── Bet.cs                    # Bet entity
│   │   ├── BettingSession.cs         # Betting session entity
│   │   └── Casino.cs                 # Casino entity
│   ├── ViewModels/
│   │   └── AccountViewModels.cs      # Login, register, external login VMs
│   ├── Views/                        # Razor views (to be created)
│   ├── wwwroot/                      # Static files (CSS, JS, images)
│   ├── appsettings.json              # Application configuration
│   └── Program.cs                    # Application entry point
└── WagerWiseJournal.sln              # Solution file
```

## Database Schema

### Tables

- **AspNetUsers**: User accounts (extends Identity)
- **Casinos**: Casino locations and information
- **BettingSessions**: Betting sessions at casinos
- **Bets**: Individual bets within sessions

### Relationships

- User → BettingSessions (One-to-Many)
- Casino → BettingSessions (One-to-Many)
- BettingSession → Bets (One-to-Many)

## Key Features Implementation

### 1. MySQL Integration with Devart
The application uses Devart.Data.MySql connector alongside Pomelo.EntityFrameworkCore.MySql for optimal MySQL performance and compatibility.

### 2. Location-Based Casino Identification
The `CasinoController` includes a `GetNearby` method that:
- Accepts GPS coordinates
- Calculates distances using the Haversine formula
- Returns nearby casinos within a specified radius
- Integrates with Google Maps for visualization (frontend implementation required)

### 3. OAuth Authentication
Multiple authentication providers are configured:
- **Local Authentication**: Email/password with Identity
- **Google OAuth**: Sign in with Google
- **Facebook OAuth**: Sign in with Facebook
- **Apple OAuth**: Ready for Apple Sign In integration

### 4. Session-Based Bet Tracking
- Create betting sessions at specific casino locations
- Add multiple bets to active sessions
- Track real-time statistics (total wagered, winnings, profit/loss)
- End sessions and maintain complete history

## Security Considerations

- All passwords are hashed using ASP.NET Core Identity's default hasher
- OAuth tokens are securely managed by the authentication middleware
- HTTPS is enforced for all production deployments
- SQL injection prevention through EF Core parameterized queries
- CSRF protection on all forms
- Secure connection strings (use environment variables or Azure Key Vault in production)

## API Endpoints

### Casino Endpoints
- `GET /Casino/Index` - List all casinos
- `GET /Casino/Create` - Create new casino form
- `POST /Casino/Create` - Submit new casino
- `GET /Casino/Details/{id}` - Casino details
- `GET /Casino/GetNearby?latitude={lat}&longitude={lon}&radiusKm={radius}` - Find nearby casinos

### Session Endpoints
- `GET /Session/Index` - List user's sessions
- `GET /Session/Create` - Create new session form
- `POST /Session/Create` - Submit new session
- `GET /Session/Details/{id}` - Session details with bets
- `POST /Session/EndSession/{id}` - End active session
- `GET /Session/Active` - Get user's active session

### Bet Endpoints
- `POST /Bet/Create` - Add bet to session
- `GET /Bet/Edit/{id}` - Edit bet form
- `POST /Bet/Edit/{id}` - Update bet
- `POST /Bet/Delete/{id}` - Delete bet

## Next Steps for Full Implementation

1. **Create Razor Views**: Implement all views for authentication, casinos, sessions, and bets
2. **Google Maps Integration**: Add JavaScript for map visualization and casino selection
3. **Apple Sign In**: Complete Apple OAuth configuration
4. **Mobile Optimization**: Enhance responsive design for mobile devices
5. **Reporting Features**: Add charts and analytics for betting history
6. **Export Functionality**: CSV/PDF export of session data
7. **Budget Tracking**: Add budget limits and alerts
8. **Multi-currency Support**: Handle different casino currencies

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.

## Support

For issues, questions, or suggestions, please open an issue on GitHub.

## Acknowledgments

- ASP.NET Core Team
- Entity Framework Core Team
- Devart for MySQL connector
- Google Maps Platform
- Bootstrap Team

