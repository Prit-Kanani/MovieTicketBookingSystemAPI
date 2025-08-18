using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Movie_Management.Middleware;
using Movie_Management_API.DTOs;
using Movie_Management_API.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Service Registrations
// ----------------------

// Controllers
builder.Services.AddControllers();

// Swagger (API docs)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (SQL Server + EF Core)
builder.Services.AddDbContext<MovieManagementContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("myConnectionString")
    ));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ----------------------
// JWT Authentication
// ----------------------

// Load JWT settings directly from config
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JWTSettings>();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Authorization
builder.Services.AddAuthorization();


// ----------------------
// App Build
// ----------------------
var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication();  // must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
