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

- Classic ASP integration (though Redis is prepared for it in .net core side)
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
