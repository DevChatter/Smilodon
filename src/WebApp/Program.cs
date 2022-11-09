using Smilodon.WebApp.Api.Admin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.MapGroup("/api/v1/admin")
    .MapAdminApiV1()
    .WithTags("Admin Endpoints");

app.UseEndpoints(x => 
{
    x.MapControllerRoute("default", "{controller}/{action}/{id?}");
});

if (app.Environment.IsDevelopment())
{
    app.UseSpa(x => 
    {
        x.UseProxyToSpaDevelopmentServer("http://localhost:1336");
    });
}
else
{
app.MapFallbackToFile("index.html");
}

app.Run();
