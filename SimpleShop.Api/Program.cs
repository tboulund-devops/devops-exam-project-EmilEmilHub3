/*
 * SimpleShop API
 * DevOps Exam Project
 * Author: Emil Rosholm
 * Datamatiker - 4th Semester
 * Spring 2026
 */

using Microsoft.EntityFrameworkCore;
using SimpleShop.Api.Data;
using SimpleShop.Api.Repositories;
using SimpleShop.Api.Services;
using SimpleShop.Api.FeatureFlags;

/// <summary>
/// Entry point for the SimpleShop Web API.
/// Configures services, middleware, database access,
/// dependency injection, and feature toggle endpoints.
/// </summary>
/// 
var builder = WebApplication.CreateBuilder(args);

#region Framework Services

// Register MVC controllers.
builder.Services.AddControllers();

// Register OpenAPI / Swagger metadata generation.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

#region Database Configuration

/// <summary>
/// Reads the database connection string from configuration.
/// Throws an exception if the value is missing.
/// </summary>
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException(
             "Connection string 'DefaultConnection' was not found.");

/// <summary>
/// Register Entity Framework Core DbContext using MySQL.
/// Version is explicitly defined for stable provider behavior.
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(cs, new MySqlServerVersion(new Version(8, 0, 36))));

#endregion

#region Repository Registration

/// <summary>
/// Registers repositories responsible for data access.
/// Scoped lifetime creates one instance per HTTP request.
/// </summary>
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

#endregion

#region Business Services Registration

/// <summary>
/// Registers business logic services.
/// These services coordinate repositories and application rules.
/// </summary>
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();

#endregion

#region Feature Toggle Services

/// <summary>
/// Singleton provider used to fetch feature states.
/// Shared across the application lifetime.
/// </summary>
builder.Services.AddSingleton<FeatureStateProvider>();

/// <summary>
/// Scoped decision service used by requests
/// to evaluate enabled/disabled features.
/// </summary>
builder.Services.AddScoped<FeatureDecisions>();

#endregion

var app = builder.Build();

#region Middleware Pipeline

/// <summary>
/// Swagger UI is only enabled in Development
/// to avoid exposing debugging tools in production.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/// <summary>
/// Serves index.html automatically when browsing root.
/// </summary>
app.UseDefaultFiles();

/// <summary>
/// Enables serving static frontend files such as:
/// HTML, CSS, JavaScript, images.
/// </summary>
app.UseStaticFiles();

#endregion

#region Endpoints

/// <summary>
/// Maps API controllers decorated with routing attributes.
/// </summary>
app.MapControllers();

/// <summary>
/// Debug endpoint used by frontend or testers
/// to inspect feature toggle state.
/// Example:
/// GET /api/feature-toggles/ProductSearch
/// </summary>
app.MapGet("/api/feature-toggles/{feature}",
    async (FeatureStateProvider featureStateProvider, string feature) =>
    {
        var result = await featureStateProvider.GetDebugInfo(feature);
        return Results.Ok(result);
    });

#endregion

/// <summary>
/// Starts the web application asynchronously.
/// </summary>
await app.RunAsync();