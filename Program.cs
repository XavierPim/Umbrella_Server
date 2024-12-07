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

// Configure the database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

// Configure HTTP Request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS must be placed after UseRouting and before UseAuthorization
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

// Modern endpoint routing: map controllers
app.MapControllers();

app.Run();
