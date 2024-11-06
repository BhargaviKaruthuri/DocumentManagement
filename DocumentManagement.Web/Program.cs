using DocumentManagement.Data;
using DocumentManagement.Service.Interface;
using DocumentManagement.Service;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connString = configuration.GetConnectionString("DefaultConnection");
    return new DbConnectionFactory(connString);
});



var logSection = builder.Configuration.GetSection("Logging");
builder.Services.AddLogging(builder =>
{
    builder.AddConfiguration(logSection);
    builder.AddConsole();
    builder.AddDebug();
});

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IDocumentService, DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Document}/{action=ListView}/{id?}");

app.Run();
