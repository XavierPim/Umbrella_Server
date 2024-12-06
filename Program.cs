using System;
using Umbrella_Server.Data;

var builder = WebApplication.CreateBuilder(args);

//Configure CORS to allow requests from React-Native App 
builder.Services.AddCors(option
    =>
{
option.AddPolicy("AllowReactApp", builder =>
builder.WithOrigins("http://localhost:3000")//Replace 
    .AllowAnyHeader()
    .AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



var app = builder.Build();

//Configure HTTP Request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMvc();
app.UseRouting();
app.UseCors("AllowReactNative");

app.MapControllers();

app.Run(); 

