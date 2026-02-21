# 🎵 MusicQuiz

A web application built with ASP.NET Core MVC (.NET 10) and MySQL.

![.NET 10.0](https://img.shields.io/badge/.NET-10.0-blueviolet)
![MySQL](https://img.shields.io/badge/Database-MySQL-blue)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 🛠 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (`global.json` targets `10.0.103`)
- [Node.js](https://nodejs.org/) (for frontend dependencies)
- [MySQL](https://dev.mysql.com/downloads/) server installed and running

---

## 📥 Setup Instructions

### 1. Clone the Repository
Download or clone the project files:

### 2. Open in Visual Studio
- Open the `.sln` file in Visual Studio 2022 (or newer).

### 3. Restore NuGet Packages
- Visual Studio should restore packages automatically.
- If not, right-click the solution in Solution Explorer → **Restore NuGet Packages**.

### 4. Install Frontend Dependencies
Navigate to the folder where your `package.json` is (for example `ClientApp/`), then run:

```bash
npm install
```

### 5. Configure Database Connection
Make sure your MySQL server is running.

Check or update the connection string in `appsettings.json`:

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "<YourSenderEmail>", 
    "SenderPassword": "<YourSenderPassword>", 
    "EnableSSL": true
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=<YourServer>;Database=MusicQuiz;User=<YourUsername>;Password=<YourPassword>;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

> ⚠️ Replace `YourSenderEmail` with your actual gmail email.
> ⚠️ Replace `YourSenderPassword` with your actual gmail app password.
NOTE: for the purpose of testing this locally, my credentials are saved in the appsettings.production.json if you wish to use them for assessment

> ⚠️ Replace `YourServer` with your actual MySQL local server, or whatever server you're using.
> ⚠️ Replace `YourUsername` with your actual MySQL username.
> ⚠️ Replace `yourpassword` with your actual MySQL password.

### 6. Set Up the Database
Open the **Package Manager Console** in Visual Studio:

- Set the default project to the one containing your `DbContext`.
- Run the following command to apply migrations and create the database:

```powershell
Update-Database
```

### 7. Run the Application
- Press `F5` or click **Start Debugging** in Visual Studio.
- The application should build and launch automatically.

---

## 🚀 You're ready to play MusicQuiz!

---
