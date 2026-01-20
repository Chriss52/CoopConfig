# CoopCloud.GeneralSettingsAndAuth

General settings management system built with **Clean Architecture** using **.NET 9** and **React 19** with React Router 7.

## 🎯 Description

Web application for centralized management of system-wide general settings, allowing definition and maintenance of configuration parameters with different data types, categories, and data sources.

## 🚀 Technologies

### Backend
- **.NET 9** - Main framework
- **Entity Framework Core 9** - ORM for SQL Server
- **MediatR** - CQRS and Mediator pattern
- **FluentValidation** - Data validation
- **Mapster** - Object mapping
- **JWT Authentication** - Authentication and authorization
- **NSwag** - API documentation

### Frontend
- **React 19** - UI library
- **React Router 7** - Routing
- **TypeScript 5.9** - Static typing
- **Vite 6** - Build tool
- **TanStack Query** - Server state management
- **React Hook Form + Zod** - Form handling
- **Tailwind CSS 4** - Styling
- **@nubeteck/shadcn** - UI components

## 📁 Project Structure

```
CoopCloud.GeneralSettingsAndAuth/
├── CoopCloud.GeneralSettingsAndAuth/        # Main project
│   ├── Features/                           # Feature modules
│   │   ├── GeneralSettings/
│   │   │   ├── API/                        # Endpoints and Slices
│   │   │   ├── Application/                # Application logic
│   │   │   │   ├── Commands/               # CQRS Commands
│   │   │   │   ├── Queries/                # CQRS Queries
│   │   │   │   ├── DTOs/                   # Data Transfer Objects
│   │   │   │   └── Repositories/           # Repository interfaces
│   │   │   ├── Domain/                     # Domain entities
│   │   │   │   └── Entities/
│   │   │   └── Infrastructure/             # Technical implementation
│   │   │       ├── Persistence/            # EF Core configuration
│   │   │       └── Repositories/           # Repository implementation
│   │   ├── Auth/                           # Authentication module
│   │   └── Shared/                         # Shared resources
│   ├── ClientApp/                          # React application
│   │   ├── api/                            # API clients
│   │   ├── components/                     # React components
│   │   ├── core/                           # Core utilities
│   │   ├── hooks/                          # Custom hooks
│   │   ├── layouts/                        # Page layouts
│   │   ├── pages/                          # Application pages
│   │   ├── providers/                      # Context providers
│   │   └── types/                          # TypeScript definitions
│   ├── Program.cs                          # Server configuration
│   ├── appsettings.json                    # Application settings
│   ├── package.json                        # Frontend dependencies
│   └── vite.config.ts                      # Vite configuration
└── Nubeteck/                               # Reusable library
    ├── Persistence/                        # Persistence utilities
    ├── Security/                           # Authentication and authorization
    └── Web/                                # Web utilities

```

## 🏗️ Architecture

The project follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles:

- **API Layer**: Endpoint definition using Minimal APIs
- **Application Layer**: Business logic with CQRS pattern using MediatR
- **Domain Layer**: Entities and business rules
- **Infrastructure Layer**: Data access and external services

## 📊 Data Model

### Main Entities

- **GeneralSetting**: System general configuration
  - `SettingId`: Unique identifier
  - `Field`: Configuration field name
  - `Value`: Current value
  - `DefaultValue`: Default value
  - `Description`: Parameter description
  - `DataTypeId`: Data type
  - `CategoryId`: Category
  - `Required`: Indicates if required
  - `Rules`: Validation rules in JSON
  - `Key`: Encryption key (optional)
  - `IsAdmin`: Indicates if admin configuration

- **GeneralSettingCategory**: Configuration categories
- **GeneralSettingDataType**: Supported data types
- **GeneralSettingSource**: Data sources for options
- **GeneralSettingOption**: Available options for select-type configurations

### Supported Data Types

1. **Number** - Numeric values
2. **ShortText** - Short text
3. **LongText** - Long text
4. **Html** - HTML content
5. **Boolean** - True/False
6. **Email** - Email addresses
7. **Options** - Single selection options
8. **MultiOptions** - Multiple selection options
9. **Editor** - Rich text editor
10. **Radio** - Radio buttons
11. **Key** - Encrypted values

## 🔧 Configuration

### Prerequisites

- .NET 9 SDK
- Node.js 18+ and npm/yarn
- SQL Server 2019+

### Database Configuration

1. Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=CoopCloudGeneralSettingsAndAuthDb;User Id=your-user;Password=your-password;"
  }
}
```

2. Run migrations (when available):

```bash
dotnet ef database update
```

### JWT Configuration

Update JWT credentials in `appsettings.json`:

```json
{
  "JwtCredentials": {
    "Key": "YOUR_SECRET_KEY_HERE_MINIMUM_32_CHARACTERS_LONG",
    "Issuer": "YourCompany",
    "Audience": "CoopCloud.GeneralSettingsAndAuth",
    "ExpiryMinutes": 1440
  }
}
```

## 🚀 Running the Application

### Development

1. **Backend**: 
```bash
dotnet run --project CoopCloud.GeneralSettingsAndAuth
```

2. **Frontend** (with HMR):
```bash
cd CoopCloud.GeneralSettingsAndAuth
npm install
npm run dev
```

The application will be available at `https://localhost:7000` (or configured port).

### Production

```bash
dotnet publish -c Release
```

The frontend will be automatically compiled and served from the backend.

## 📡 API Endpoints

### General Settings

- `GET /api/general-settings` - Gets all settings
- `GET /api/general-settings/{id}` - Gets a setting by ID
- `GET /api/general-settings/categories` - Gets all categories
- `GET /api/general-settings/category/{categoryId}` - Gets settings by category
- `PUT /api/general-settings/{id}/value` - Updates a setting value
- `PUT /api/general-settings` - Updates multiple settings
- `DELETE /api/general-settings/{id}` - Deletes a setting
- `DELETE /api/general-settings/range` - Deletes multiple settings

## 🔐 Authentication

The system uses JWT (JSON Web Tokens) for authentication. All endpoints require authentication except the login endpoint.

## 🧪 Testing

```bash
dotnet test
```

## 📝 License

[Specify license]

## 👥 Authors

- Nubeteck Team

## 🤝 Contributing

[Contributing instructions if applicable]
