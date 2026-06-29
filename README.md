# VendorHub

A full-stack vendor management and quotation system built with **ASP.NET Core 9.0** and **Blazor Server**.

## Features

- **Admin Dashboard** - Manage vendors, quotation requests, and track activity
- **Vendor Portal** - Vendors can view assigned quotations, respond, and manage profiles
- **Quotation Management** - Create, assign, compare, and respond to quotation requests
- **Authentication** - Secure login with BCrypt password hashing and JWT tokens
- **Email Notifications** - SMTP-based email service for invitations and updates
- **Activity Logging** - Track all user and system activities

## Tech Stack

- **Backend:** ASP.NET Core 9.0 (MVC + Blazor Server)
- **Database:** PostgreSQL (Neon) with Entity Framework Core
- **Authentication:** JWT + BCrypt
- **Frontend:** Blazor Server + Bootstrap
- **Email:** SMTP (Gmail)

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- PostgreSQL database (or [Neon](https://neon.tech/) account)

## Setup

1. Clone the repository:
```bash
git clone https://github.com/yourusername/Teyzix-core-task-2-Vendor-Hub.git
cd Teyzix-core-task-2-Vendor-Hub
```

2. Create the `.env` file inside `VendorHub/`:
```bash
cp VendorHub/.env.example VendorHub/.env
```

3. Update `.env` with your credentials:
```
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_SENDER_EMAIL=your-email@gmail.com
SMTP_SENDER_PASSWORD=your-app-password
SMTP_SENDER_NAME=Your Name
```

4. Update `VendorHub/appsettings.json` with your database connection string and JWT key.

5. Run the application:
```bash
cd VendorHub
dotnet run
```

6. Open `http://localhost:5143` in your browser.

## Project Structure

```
VendorHub/
├── Components/          # Blazor components (Admin, Vendor, Shared)
├── Controllers/         # MVC Controllers
├── Data/                # DbContext
├── Models/              # Entity classes and Repositories
├── Services/            # Email and Auth services
├── Views/               # Razor views
├── wwwroot/             # Static files (CSS, JS, libs)
├── .env                 # Environment variables (NOT committed)
├── .env.example         # Template for .env
└── appsettings.json     # App configuration
```

## Environment Variables

| Variable | Description |
|----------|-------------|
| `SMTP_HOST` | SMTP server host |
| `SMTP_PORT` | SMTP server port |
| `SMTP_SENDER_EMAIL` | Sender email address |
| `SMTP_SENDER_PASSWORD` | App password for SMTP |
| `SMTP_SENDER_NAME` | Display name for sender |

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
