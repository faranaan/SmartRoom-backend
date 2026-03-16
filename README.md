# 🏨 SmartRoom - Campus Booking System API

![Build](https://img.shields.io/badge/build-passing-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue)
![License](https://img.shields.io/badge/license-MIT-green)

SmartRoom is a RESTful backend API designed to manage campus room bookings digitally.  
This system replaces manual booking processes, prevents schedule conflicts, and simplifies administrative workflows.

---

## ✨ Features

1. Authentication & Authorization
   - User registration & login (Student / Admin)
   - JWT-based authentication
   - Role-based access control

2. Room Management (CRUD)
   - Create, read, update, and delete room data
   - Room availability status

3. Booking System
   - Room booking with date & time validation
   - Conflict detection to prevent double booking
   - Approval workflow (Pending, Approved, Rejected)
   - User booking history (My Bookings)

---

## 🛠 Tech Stack

- Framework: ASP.NET Core Web API (.NET 8)
- Language: C#
- Database: PostgreSQL
- ORM: Entity Framework Core (Code First)
- Authentication: JWT Bearer & BCrypt
- Documentation: Swagger / OpenAPI

---

## 🚀 Installation

### Prerequisites
- .NET SDK 8.0 or later
- PostgreSQL Server (Local or Docker)
- Git

### Clone Repository

    git clone https://github.com/your-username/SmartRoom-backend.git
    cd SmartRoom-backend

---

## 🗄️ Database Setup

1. Create an empty PostgreSQL database  
   Example: campus_booking_db

2. Configure the connection string in appsettings.json  
   See Environment Variables section below

### Run Migrations

    dotnet ef database update

---

## 💻 Usage

### Start the Server

    dotnet run

### Access API Documentation

    http://localhost:5xxx/swagger

(Port may vary depending on your local setup)

### Testing Flow

1. Register user  
   POST /api/Auth/register

2. Login user  
   POST /api/Auth/login

3. Copy JWT token

4. Click the lock icon in Swagger and enter  

    Bearer <your_token>

5. Access protected endpoints (Rooms / Bookings)

---

## 🌿 Environment Variables

Configure the following in appsettings.json:

    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=campus_booking_db;Username=postgres;Password=your_password"
      },
      "Jwt": {
        "Key": "Replace_With_A_Secret_Key_Minimum_32_Characters",
        "Issuer": "CampusBooking.API",
        "Audience": "CampusBooking.Client"
      }
    }

Do not commit real credentials.  
Use User Secrets or environment variables for production environments.

---

## 🤝 Contributing

1. Fork this repository
2. Create a feature branch

    git checkout -b feature/NewFeature

3. Commit your changes

    git commit -m "feat: add new feature"

4. Push to branch

    git push origin feature/NewFeature

5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License.  
Free to use and modify for educational purposes.

---

## ✍️ Author

Muhammad Far'An
Backend Developer  
GitHub: https://github.com/faranaan
