# 🎬 Movie Ticket Booking – Backend API (ASP.NET Core Web API)

This is the **backend API for the Movie Ticket Booking System**, built with **ASP.NET Core Web API (.NET 8)**.  
It provides secure endpoints for managing movies, genres, theatres, screens, showtimes, bookings, and authentication.  
This API is consumed by the [MovieTicketBooking_Frontend](https://github.com/Prit-Kanani/MovieTicketBooking_Frontend) project.

---

## ✨ Features

### 👤 User
- Register / Login with JWT authentication
- Browse movies, genres, and available showtimes
- Book tickets and view booking history
- Role-based access control (User / Admin)

### 🛠️ Admin
- Manage Movies, Theatres, Screens, and Showtimes
- Manage Genres and assign them to movies
- View all bookings and user activity
- Secure API access with JWT tokens

---

## 🏗️ Tech Stack

- **Framework:** ASP.NET Core Web API (.NET 8)
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT (JSON Web Token)
- **Error Handling:** Custom middleware (`ExceptionMiddleware.cs`)
- **IDE:** Visual Studio 2022

---

## 📂 Project Structure
Movie_Management_API/
├── Controllers/
│ ├── AuthController.cs # Handles login, register, JWT
│ ├── BookingAPIController.cs # Booking endpoints
│ ├── GenresController.cs # Genre management
│ ├── MovieAPIController.cs # Movie management
│ └── TheatreAPIController.cs # Theatre, screens, and shows
├── DTOs/ # Data Transfer Objects
├── Models/ # Entity models
├── Middleware/ExceptionMiddleware.cs
├── appsettings.json # Configurations
├── Program.cs # Entry point
└── Movie_Management_API.csproj


---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server
- Visual Studio 2022

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/Prit-Kanani/MovieTicketBooking_API.git
cd Movie_Management_API
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MovieDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

Apply migrations & update database:
dotnet ef database update

Run the API:
dotnet run

Open Swagger UI:
https://localhost:5001/swagger


🔑 Authentication

Register a new user via /api/Auth/register

Login via /api/Auth/login → receive JWT

Use the token in Swagger or API client:
Authorization: Bearer <your_token>


📌 Roadmap

 Add payment gateway integration

 Add movie reviews & ratings

 Implement seat selection in bookings

 Caching for performance

 🤝 Contributing

Pull requests are welcome. For major changes, open an issue first to discuss what you’d like to change.

📜 License

This project is licensed under the MIT License – feel free to use and modify.


---

👉 Do you want me to **save this `README.md` directly inside your `Movie_Management_API` folder** (so it’s ready to commit), or will you copy it manually into your repo? ​:contentReference[oaicite:0]{index=0}​
