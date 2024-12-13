using System;
using Umbrella_Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow requests from React-Native App 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000") // Replace with your React Native development URL
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// Register DbContext with NetTopologySuite for GEOGRAPHY support
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.UseNetTopologySuite()
    )
);

// Register controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Add HTTP Strict Transport Security headers
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS must be placed after UseRouting and before UseAuthorization
app.UseCors("AllowReactApp");

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Endpoint routing for controllers
app.MapControllers();

app.Run();
