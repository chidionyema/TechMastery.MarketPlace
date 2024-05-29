using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true; // Enables logging of Personally Identifiable Information (PII). Use only in development.

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Or appropriate level based on your needs

var jwtSettings = builder.Configuration.GetSection("JwtSettings");


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
     //   options.Authority = "https://usermanagement-service.local:5001"; // Assuming 5001 is the port of your OpenID configuration endpoint

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true, // Set to true if you specify audiences
            ValidAudience = jwtSettings["Audience"], // Specify if validating audiences
            // ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"], // The issuer must match the authority
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])), // Use the same key as your authentication service
            ValidateLifetime = true, // Validates token expiration
            ClockSkew = TimeSpan.Zero // Optional: reduce or eliminate clock skew allowance
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("JwtBearer");
                logger.LogError($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("JwtBearer");
                logger.LogInformation("Token successfully validated.");
                return Task.CompletedTask;
            }
        };
    });

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow5000",
        builder => builder.WithOrigins("http://usermanagement-service.local:5000", "https://usermanagement-service.local:5001")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Data Protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/app/keys"))
    .SetApplicationName("TechMastery.SecureService")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Allow5000");

app.UseHttpsRedirection();
app.UseHsts(); // Be cautious with HSTS in development environments.
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
