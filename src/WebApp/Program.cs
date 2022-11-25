using Microsoft.EntityFrameworkCore;
using Smilodon.Infrastructure.Persistence;
using Smilodon.WebApp.Api.Admin;
using Smilodon.WebApp.Api.Webfinger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Add the database context to the DI container
builder.Services.AddDbContext<SmilodonDbContext>(options =>
    options
        .UseNpgsql("Host=localhost;Database=smilodon;User Id=smilodon;Password=smilodon;")
        .UseSnakeCaseNamingConvention());

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.MapGroup("/api/v1/admin")
    .MapAdminApiV1()
    .WithTags("Admin Endpoints");

app.MapGroup("/.well-known/")
    .MapWellKnownApi()
    .WithTags("Well Known Endpoints");

app.Run();
