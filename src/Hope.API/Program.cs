using Hope.Infrastructure.Data;
using Hope.Infrastructure.Data.Seed;
using Hope.Infrastructure.Interfaces;
using Hope.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// POSTGRESQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine(Directory.GetCurrentDirectory());

using var scope = app.Services.CreateScope(); //Using asegura que estos servicios se borren despues de usarlos
var service = scope.ServiceProvider;
try
{
    var context = service.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedAll(context);
} catch (Exception ex) 
{
    Console.WriteLine("An error occurred during migration" + ex.Message);
}

app.Run();
