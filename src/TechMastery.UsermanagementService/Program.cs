using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechMastery.UsermanagementService;
using Serilog;
using AspNetCoreRateLimit; // For rate limiting
using Microsoft.AspNetCore.DataProtection;
using TechMastery.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client; // For UI and detailed response
using Serilog.Events;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Register services with the DI container
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddMessagingServices(builder.Configuration);
// Use Serilog for structured logging for better monitoring and diagnostics
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .Enrich.FromLogContext());

// Construct the database connection string dynamically
var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "myappdb";
var username = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "mysecretpassword";

var identityDbConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(identityDbConnectionString, b =>
        b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


// Enhanced security for Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Enhanced password security
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings") ?? throw new InvalidOperationException("missing JwtSettings config");
var authSettings = builder.Configuration.GetSection("Authentication") ?? throw new InvalidOperationException("missing Authentication config");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ValidateIssuer = true,
            ValidIssuer = "https://usermanagement-service.local:5001",
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"]
        };
    }).AddGoogle(options =>
    {
        options.ClientId = authSettings["Google:ClientId"];
        options.ClientSecret = authSettings["Google:ClientSecret"];
    })
    .AddFacebook(options =>
    {
        options.ClientId = authSettings["Facebook:ClientId"];
        options.ClientSecret = authSettings["Facebook:ClientSecret"];
    });

// Rate Limiting and Throttling
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add Health Checks for the application and its dependencies
// Add Health Checks for the application and its dependencies
builder.Services.AddHealthChecks()
    .AddNpgSql(identityDbConnectionString, name: "PostgreSQL", tags: new[] { "db", "sql", "postgres" })
    .AddUrlGroup(new Uri("https://usermanagement-service.local:5001"), name: "External API", tags: new[] { "external", "api" });


// API Controllers
builder.Services.AddControllers();

// Swagger for API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow5001",
        builder => builder.WithOrigins("https://secure-service.local:5003","http://secure-service.local:5002","https://localhost:5003")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys"))
    .SetApplicationName("TechMastery.UsermanagementService")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();
var serviceProvider = app.Services;

// Map Health Checks endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Use Serilog for request logging
app.UseSerilogRequestLogging(options =>
{
    // Customize the request logging options as needed
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        // Set the log level based on the status code of the response
        if (httpContext.Response.StatusCode >= 500)
        {
            return LogEventLevel.Error;
        }
        else if (httpContext.Response.StatusCode >= 400)
        {
            return LogEventLevel.Warning;
        }
        else
        {
            return LogEventLevel.Information;
        }
    };
});
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// Middleware registration
app.UseSecurityHeaders();
app.UseGlobalExceptionHandling();

app.UseCors("Allow5001");
app.UseHttpsRedirection();
app.UseHsts(); // Be cautious with HSTS in development environments.
app.UseAuthentication();
app.UseAuthorization();
// Rate Limiting
app.UseIpRateLimiting();
app.MapControllers();
app.Run();

public partial class Program { }