<p align="center">
  <img alt="IngressQuote — Vendor Management & Quotation System" src="./VendorHub/wwwroot/logo.png" width="100" style="border-radius: 16px;">
</p>

<h1 align="center">Ingress<span style="color:#00b8a9">Quote</span></h1>

<p align="center">
  <strong>Manage Vendors. Track Quotations. Streamline Procurement. — The Smart Vendor Management System Built for Modern Businesses</strong>
</p>

<p align="center">
  <a href="#">
    <img src="https://img.shields.io/badge/🚀_Live_Demo-ingressquote.vercel.app-00b8a9?style=for-the-badge&labelColor=0d1117" alt="Live Demo">
  </a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Blazor_Server-ASP.NET_Core_9-512BD4?style=flat&logo=blazor&labelColor=0d1117">
  <img src="https://img.shields.io/badge/C%23-.NET_9-239120?style=flat&logo=csharp&labelColor=0d1117">
  <img src="https://img.shields.io/badge/PostgreSQL-NeonDB-4169E1?style=flat&logo=postgresql&labelColor=0d1117">
  <img src="https://img.shields.io/badge/EF_Core-Code_First-512BD4?style=flat&logo=dotnet&labelColor=0d1117">
  <img src="https://img.shields.io/badge/JWT-Auth-000000?style=flat&logo=jsonwebtokens&labelColor=0d1117">
  <img src="https://img.shields.io/badge/BCrypt-Password_Hashing-00b8a9?style=flat&labelColor=0d1117">
  <img src="https://img.shields.io/badge/SMTP-Email_Service-EA4335?style=flat&logo=gmail&labelColor=0d1117">
  <img src="https://img.shields.io/badge/ApexCharts-Data_Viz-00b8a9?style=flat&logo=chartdotjs&labelColor=0d1117">
</p>

<br>

---

<!-- SEO KEYWORDS -->
<!--
ingressquote, vendor management system, quotation management system, blazor server project, asp.net core 9, full stack web application, procurement software, supplier management tool, quotation tracking dashboard, vendor invitation system, email notification procurement, blazor server dashboard with charts, internship project dotnet, full stack developer Pakistan, Teyzix Core internship, vendor quotation management, cost tracking tool, B2B procurement platform, PostgreSQL NeonDB EF Core project, apexcharts dashboard, dark light mode blazor app, activity logging system
-->

---

## 📌 Overview

**IngressQuote** is a full-stack vendor management and quotation system built with **ASP.NET Core 9.0** and **Blazor Server**. Designed for businesses that need a centralized, professional way to manage supplier relationships and procurement workflows — from onboarding vendors via email invitation to tracking quotation responses and comparing submissions side by side.

Instead of scattered spreadsheets and email threads, IngressQuote brings the entire procurement pipeline into one structured dashboard. Admins manage the full system while vendors get their own dedicated portal to view assigned quotations, respond with pricing, and manage their profiles.

The platform supports dual-portal access — **Admin** and **Vendor** — each with role-specific dashboards, activity logs, and data visualization powered by ApexCharts.

> Built to reflect how real procurement teams actually operate — structured, trackable, and fully auditable.

---

## ❗ Problem Statement

Most small-to-medium businesses still rely on email and Excel to manage vendor communications. As supplier count grows, three unavoidable problems emerge:

- **No centralized vendor database** — supplier contacts, history, and status are scattered across inboxes and files
- **Manual quotation tracking** — following up on submissions, comparing responses, and updating statuses wastes hours of administrative work
- **Zero procurement visibility** — no way to see patterns, track vendor performance, or audit who did what and when

**How IngressQuote solves this:**
- Vendors are onboarded via **secure email invitations** — no manual account creation needed
- Quotation requests are created and **assigned to specific vendors** directly from the admin panel
- Vendors respond through their own portal — responses are tracked, timestamped, and visible to admins instantly
- **Activity logging** creates a full audit trail of every action in the system
- **Interactive dashboards** give both admins and vendors real-time visual insight into quotation status, vendor performance, and monthly activity trends

---

## ✨ Key Features

| Feature | Description |
|---|---|
| 🏢 **Vendor Directory** | Add, edit, search, and manage all supplier information in one place |
| 📧 **Email Invitations** | Invite vendors via SMTP — they receive a secure link to set up their account |
| 📋 **Quotation Requests** | Create structured requests and assign them to specific vendors |
| 📥 **Vendor Responses** | Vendors submit pricing through their dedicated portal; responses tracked with status management |
| 📊 **Interactive Charts** | Donut chart for quotation status overview, bar chart for top vendors, line chart for monthly activity |
| 🔔 **Activity Logging** | Full audit trail — every user action and system event is recorded and viewable |
| 🌗 **Dark / Light Mode** | Fully themed toggle with persistent preference |
| 🔐 **JWT Authentication** | Secure login with BCrypt password hashing and token-based route protection |
| 👥 **Dual Portal Access** | Separate Admin and Vendor portals with role-specific views and permissions |
| 📱 **Responsive UI** | Clean layout across desktop and tablet with collapsible sidebar |

---

## 🛠️ Tech Stack

**Frontend**
- Blazor Server (ASP.NET Core 9.0)
- Bootstrap 5
- ApexCharts (dashboard data visualization)
- Custom dark / light theme system

**Backend**
- ASP.NET Core 9.0 (MVC + Blazor Server)
- Entity Framework Core (Code First)
- PostgreSQL via Neon cloud database
- JWT (token-based auth) + BCrypt (password hashing)
- SMTP email service (Gmail)

**Architecture**
- Repository Pattern
- Dependency Injection
- Session-based role management (Admin / Vendor)
- Protected route components via Blazor auth state

---

## 🗂️ Project Structure

```
IngressQuote/
└── VendorHub/
    ├── Components/              # Blazor components
    │   ├── Admin/               # Admin portal pages and components
    │   ├── Vendor/              # Vendor portal pages and components
    │   └── Shared/              # Shared layout, navbar, sidebar
    ├── Controllers/             # MVC Controllers (auth, invitations)
    ├── Data/
    │   └── AppDbContext.cs      # EF Core DbContext
    ├── Models/                  # Entity classes
    │   └── Repositories/        # Repository interfaces and implementations
    ├── Services/
    │   ├── EmailService.cs      # SMTP email service
    │   └── JwtService.cs        # JWT generation and validation
    ├── Views/                   # Razor views (auth pages)
    ├── wwwroot/                 # Static files (CSS, JS, libs)
    ├── .env                     # Environment variables (NOT committed)
    ├── .env.example             # Template for environment setup
    ├── appsettings.json         # App configuration
    └── Program.cs               # DI registration and middleware
```

---

## 🌐 API & Route Structure

### Auth (MVC Controllers)
| Method | Path | Description | Access |
|---|---|---|---|
| GET/POST | /Auth/Login | Login page | Public |
| GET/POST | /Auth/Register | Register via invitation token | Public |
| GET | /Auth/Logout | Clear session and redirect | Private |

### Admin Portal (Blazor)
| Route | Description | Access |
|---|---|---|
| /admin/dashboard | Stats, charts, activity overview | Admin |
| /admin/vendors | Vendor CRUD + invitation management | Admin |
| /admin/quotations | Create and manage quotation requests | Admin |
| /admin/responses | View all vendor responses | Admin |
| /admin/activity | Full system activity log | Admin |

### Vendor Portal (Blazor)
| Route | Description | Access |
|---|---|---|
| /vendor/dashboard | Personal stats and charts | Vendor |
| /vendor/quotations | View assigned quotation requests | Vendor |
| /vendor/responses | Submit and manage responses | Vendor |
| /vendor/profile | Edit profile information | Vendor |

---

## 📊 Dashboard & Charts

Both Admin and Vendor dashboards include a **3-row visual layout**:

**Row 1 — Stat Cards**
Quick summary numbers: total vendors, total quotations, pending approvals, approved this month

**Row 2 — Side-by-Side Charts**
- **Left:** Donut chart — Quotation Status Overview (Approved / Pending / Rejected)
- **Right:** Horizontal bar chart — Top Vendors by Approvals

**Row 3 — Full Width Chart**
- Area/line chart — Monthly Quotation Activity (Jan through Dec)

All charts are built with **ApexCharts** and respond to dark/light mode toggle automatically.

---

## 🔐 Authentication & Invitation Flow

1. Admin creates a vendor record and sends an **email invitation**
2. Vendor receives a secure tokenized link via SMTP
3. Vendor clicks the link and completes registration (sets password)
4. JWT is issued on login — stored in session for route protection
5. Role (Admin / Vendor) is read from JWT claims on every request
6. Blazor components check auth state via `ProtectedSessionStorage`

---

## ⚙️ How to Run Locally

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- PostgreSQL database or [Neon](https://neon.tech/) free account
- Gmail account with App Password enabled for SMTP

### 1. Clone the repository
```bash
git clone https://github.com/MuhammadOkasha004/IngressQuote.git
cd IngressQuote
```

### 2. Create environment file
```bash
cp VendorHub/.env.example VendorHub/.env
```

### 3. Configure `.env`
```env
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_SENDER_EMAIL=your-email@gmail.com
SMTP_SENDER_PASSWORD=your-app-password
SMTP_SENDER_NAME=IngressQuote
```

### 4. Configure `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-neon-host;Database=ingressquote;Username=your-user;Password=your-password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "IngressQuote",
    "Audience": "IngressQuoteUsers"
  }
}
```

### 5. Apply database migrations
```bash
cd VendorHub
dotnet ef database update
```

### 6. Run the application
```bash
dotnet run
```

Open **http://localhost:5143** in your browser.

---

## 🌱 Environment Variables Reference

| Variable | Description |
|---|---|
| `SMTP_HOST` | SMTP server host (e.g. smtp.gmail.com) |
| `SMTP_PORT` | SMTP port (587 for TLS) |
| `SMTP_SENDER_EMAIL` | Sender email address |
| `SMTP_SENDER_PASSWORD` | Gmail App Password |
| `SMTP_SENDER_NAME` | Display name shown in emails |

---

## 🔮 Future Roadmap

- [ ] Quotation deadline tracker with auto-close on expiry
- [ ] Smart cost comparison across vendor responses per item
- [ ] PDF procurement report export
- [ ] Multi-role support (Procurement Manager, Viewer, Super Admin)
- [ ] Real-time notifications via SignalR
- [ ] Vendor performance scoring and ranking system
- [ ] Export to Excel

---

## 👤 Author

**Muhammad Okasha**  
Software Engineering Student · ASP.NET Core Developer · Full Stack Developer · TEYZIX CORE

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-0077B5?style=flat&logo=linkedin&labelColor=0d1117)](https://www.linkedin.com/in/muhammadokasha004)
[![GitHub](https://img.shields.io/badge/GitHub-Follow-333?style=flat&logo=github&labelColor=0d1117)](https://github.com/MuhammadOkasha004)

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](./LICENSE) file for details.

---

<p align="center">
  <strong>⭐ Found IngressQuote useful or impressive? Drop a star!</strong><br>
  <sub>Built from scratch during my internship at TEYZIX CORE — every feature thought through, every bug fixed.<br>
  A star costs nothing but means everything. 🙏</sub>
</p>

<p align="center">
  <a href="https://github.com/MuhammadOkasha004/IngressQuote">
    <img src="https://img.shields.io/badge/⭐_Star_this_repo-Show_some_love-00b8a9?style=for-the-badge&labelColor=0d1117" alt="Star this repo">
  </a>
</p>
