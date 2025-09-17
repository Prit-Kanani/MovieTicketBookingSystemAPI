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

