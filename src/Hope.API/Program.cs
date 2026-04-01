using FluentValidation;
using Hope.API.Middleware;
using Hope.Application.Interfaces;
using Hope.Application.Services;
using Hope.Application.Validators;
using Hope.Domain.Models;
using Hope.Infrastructure.Data;
using Hope.Infrastructure.Data.Seed;
using Hope.Infrastructure.Interfaces;
using Hope.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// INFRASTRUCTURE
// POSTGRESQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityCore<User>(opt =>
{
    opt.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//APPLICATION
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IMealService, MealService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt => {
    var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found - Program.cs");
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddValidatorsFromAssemblyContaining<UserInsertDtoValidator>(); //Usando la ubicacion UserInsertDtoValidator registra automaticamente el resto de validadores

//API
builder.Services.AddControllers();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope(); //Using asegura que estos servicios se borren despues de usarlos
var service = scope.ServiceProvider;
try
{
    var context = service.GetRequiredService<ApplicationDbContext>();
    var userManager = service.GetRequiredService<UserManager<User>>();
    var roleManager = service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await context.Database.MigrateAsync();
    await Seed.SeedAll(context, userManager, roleManager);
} catch (Exception ex) 
{
    Console.WriteLine("An error occurred during migration" + ex.Message);
}

app.Run();
