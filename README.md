# 🌦️ WeatherForecast API

**This project contains a sample ASP.NET Core app. This app is an example of the article I produced for the Telerik Blog (telerik.com/blogs).**

Welcome to the **WeatherForecast API**! This is a minimal ASP.NET Core API application that provides weather forecasts. 
The application uses SQL Server for its database and leverages in-memory caching with `Microsoft.EntityFrameworkCore.InMemory`. It also uses Entity Framework Core (EF Core) for data access.

## 🚀 Getting Started

These instructions will help you set up and run the project on your local machine for development and testing purposes.

### Prerequisites

Ensure you have the following installed:

- .NET SDK
- SQL Server

### Installation

1. **Clone the repository:**

   ```bash
   git clone https://github.com/zangassis/weather-forecast-api.git
   cd WeatherForecastAPI
   ```

2. **Restore the dependencies:**

   ```bash
   dotnet restore
   ```

3. **Update the database:**

   Ensure your SQL Server is running and the connection string in `appsettings.json` is correct.

   ```bash
   dotnet ef database update
   ```

### Running the Application

To run the application, use the following command:

```bash
dotnet run
```

## 🗄️ Database Configuration

The application uses SQL Server as the primary database. The connection string can be found and configured in the `appsettings.json` file.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_CONFIGURATION;Database=WeatherForecastDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## 💾 In-Memory Caching

The application utilizes in-memory caching for better performance. This is set up using `Microsoft.EntityFrameworkCore.InMemory`.

## 🛠️ EF Core

Entity Framework Core (EF Core) is used for database operations. The models and context are defined to interact with SQL Server efficiently.

### Migrations

To add a new migration, use the following command:

```bash
dotnet ef migrations add MigrationName
```

To update the database with the latest migrations:

```bash
dotnet ef database update
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
