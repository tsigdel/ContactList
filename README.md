
# Contact List Application

## Overview

**ASP.NET Core**, **cloud deployment (Azure)**, **Redis caching**, **database schema design**, and **unit testing**. It is a basic contact list application that allows users to register, log in, and manage their contacts with full CRUD functionality.

> ⚠️ **Note:** The Classic ASP portion of the application has not been implemented. However, Redis has been integrated for session management, and is configured in a way that supports interoperability with Classic ASP if implemented in the future.

## Features

### ✅ Implemented Features

- **User Registration:** New users can create an account with a username and password.
- **Login / Logout:** Users can log in and securely log out.
- **Contact List View:** Authenticated users can view their saved contacts.
- **CRUD Operations:** Full Create, Read, Update, Delete support for contacts.
- **Search Functionality:** Contacts can be searched by name or other information.
- **Redis Cache:** Used to store sessions, making it extensible to share between ASP.NET and Classic ASP.
- **Unit Testing:** Key services and controllers are covered with NUnit + Moq.
- **Azure:** The app is deployable to any major cloud provider.
- **Modern ASP.NET Patterns:** Uses dependency injection, claim-based authentication, and Entity Framework Core.

### 🚫 Not Implemented

- Classic ASP integration (though Redis is prepared for it in .NET Core side)
- Password reset functionality

## Tech Stack

- **Backend:** ASP.NET Core MVC
- **Frontend:** Razor Views (Bootstrap)
- **Database:** SQL Server / Azure SQL
- **Authentication:** Cookie-based with claims
- **Session Caching:** Redis
- **Testing:** NUnit, Moq
- **Deployment:** Azure ready

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet)
- SQL Server or Azure SQL instance
- Redis server running locally or remotely
- Visual Studio or VS Code

### Setup Instructions

1. Clone the repository:
   ```bash
   git clone [https://github.com/your-username/contact-list-app.git](https://github.com/tsigdel/ContactList)
   cd contact-list-app
   ```

2. Install the required NuGet packages:
   ```bash
   dotnet restore
   ```

3. Configure Production (Azure)

In production (Azure), the connection strings are managed through the Application Settings in the Azure portal. The values for DefaultConnection and RedisConnection should be set as follows:

SQL Server Connection String: Set the connection string for your SQL Server under Application Settings -> Connection Strings in Azure.

Redis Connection String: Set the Redis connection string for Redis Cache in Application Settings -> Connection Strings.

Azure will automatically use these environment variables in production, overriding the settings in appsettings.json.

4. Start the application:
   ```bash
   dotnet run
   ```

5. The application should be accessible at `http://localhost:5000`.

## Deployment

This app is configured for deployment to **Azure**. It includes a GitHub Actions workflow and an Azure pipeline configuration to automate the deployment process.

### Deploying to Azure

1. Create an Azure Web App.
2. Set up continuous deployment with GitHub or Azure Pipelines.
3. Configure Redis caching in the Azure portal if not already set up.
4. Deploy app.

## Testing

This app uses **NUnit** and **Moq** for unit testing. Run the tests using the following command:

```bash
dotnet test
```

## Contributing

Feel free to fork the project, open issues, and submit pull requests. Contributions are welcome!

## License

This project is licensed under the MIT License.
