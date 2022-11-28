using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Smilodon.Infrastructure.Persistence;
using Smilodon.WebApp.Api.Admin;
using Smilodon.WebApp.Api.Webfinger;

var builder = WebApplication.CreateBuilder(args);

// this should really be in the app config instead...
const string serviceName = "Smilodon.WebApp";
const string serviceVersion = "0.0.1";

// Add services to the container.

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .AddConsoleExporter()
        .AddOtlpExporter(opt =>
        {
            opt.Protocol = OtlpExportProtocol.HttpProtobuf;
        })
        .AddSource(serviceName)
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation();
});

builder.Services.AddControllersWithViews();

// Add the database context to the DI container
builder.Services.AddDbContext<SmilodonDbContext>(options =>
    options
        .UseNpgsql("Host=localhost; Database=smilodon; User Id=smilodon; Password=smilodon;")
        .UseSnakeCaseNamingConvention());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

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
