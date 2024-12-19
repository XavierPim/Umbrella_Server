using System;
using Umbrella_Server.Data;
using Microsoft.EntityFrameworkCore;
using Umbrella_Server.Data.Repositories.User;

var builder = WebApplication.CreateBuilder(args);

// 🔥 CORS Configuration (allow localhost in development, production URL in production)
var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[] { "http://localhost:3000" }
    : new[] { "https://your-production-domain.com" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 🔥 Register DbContext for EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.UseNetTopologySuite()
    )
);

// 🔥 Register controllers and JSON options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// 🔥 Dependency Injection for repositories (optional if you use repositories)
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// 🔥 Production Error Handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseRouting();

// 🔥 Enable CORS for frontend apps
app.UseCors("AllowReactApp");

// 🔥 Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// 🔥 Map Controllers
app.MapControllers();

app.Run();
