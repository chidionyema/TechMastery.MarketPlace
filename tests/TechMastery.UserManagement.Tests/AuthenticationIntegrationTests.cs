using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TechMastery.UsermanagementService;
using TechMastery.UsermanagementService.Controllers;

public class AuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>> // Use Program instead of Startup
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Find and remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add the ApplicationDbContext using an in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // Optionally, mock or replace other services as needed
            });
        });
    }

    [Fact]
    public async Task Register_User_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newUser = new { Email = "test@example.com", Password = "Test123$" };

        // Act
        var response = await client.PostAsJsonAsync("/Authentication/register", newUser);

        // Assert
        response.EnsureSuccessStatusCode();
        // You can add more detailed assertions here
    }

    [Fact]
    public async Task Login_User_ReturnsToken()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = "test@example.com";
        var password = "Test123$";

        // Ensure the user is registered
        await RegisterUser(client, email, password);
        // Confirm the user's email
        await ConfirmEmailDirectly(client, email);


        var loginInfo = new { Email = email, Password = password };

        // Act
        var response = await client.PostAsJsonAsync("/Authentication/login", loginInfo);

        // Assert
        response.EnsureSuccessStatusCode();
        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(tokenResponse?.Token);
        // Further assertions can be added here
    }

    [Fact]
    public async Task Invalid_Username_User_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = "test@example.com";
        var password = "Test123$";

        // Ensure the user is registered
        await RegisterUser(client, email, password);
        // Confirm the user's email
        await ConfirmEmailDirectly(client, email);


        var loginInfo = new { Email = "invalid@example.com", Password = password };

        // Act
        var response = await client.PostAsJsonAsync("/Authentication/login", loginInfo);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    }
    [Fact]
    public async Task Invalid_Password_User_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = "test@example.com";
        var password = "Test123$";

        // Ensure the user is registered
        await RegisterUser(client, email, password);
        // Confirm the user's email
        await ConfirmEmailDirectly(client, email);


        var loginInfo = new { Email = email, Password = "invalidPassword" };

        // Act
        var response = await client.PostAsJsonAsync("/Authentication/login", loginInfo);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

    }

    [Fact]
    public async Task Login_User_Without_Registration_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = "test@example.com";
        var password = "Test123$";

        var loginInfo = new { Email = email, Password = password };

        // Act
        var response = await client.PostAsJsonAsync("/Authentication/login", loginInfo);

        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        Assert.Contains("Invalid login attempt.", content);
    }

    [Fact]
    public async Task Login_User_Without_Confirmation_Returns401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var email = "test@example.com";
        var password = "Test123$";

        // Ensure the user is registered
        await RegisterUser(client, email, password);


        var loginInfo = new { Email = email, Password = password };

        // Act
        var response = await client.PostAsJsonAsync("/Authentication/login", loginInfo);

        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        Assert.Contains("You must have a confirmed email to log in.", content);

    }

    [Fact]
    public async Task ResetPassword_ReturnsExpectedResult_WithValidToken()
    {
        // Arrange
        var resetPasswordModel = new ResetPasswordDto
        {
            Email = "user@example.com",
            Token = "ValidTokenHere", // You need to set up a way to generate or mock a valid token
            NewPassword = "NewPassword123!"
        };
        var client = _factory.CreateClient();
        // Act
        var response = await client.PostAsJsonAsync("/Authentication/reset-password", resetPasswordModel);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Your password has been reset successfully.", content);
    }


    [Fact]
    public async Task ForgotPassword_ReturnsExpectedResult()
    {
        // Arrange
        var forgotPasswordModel = new ForgotPasswordDto { Email = "user@example.com" };
        var client = _factory.CreateClient();
        // Act
        var response = await client.PostAsJsonAsync("/Authentication/forgot-password", forgotPasswordModel);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("If your email is in our system, you will receive a password reset email.", content);
    }
    private async Task ConfirmEmailDirectly(HttpClient client, string email)
    {
        // Assuming you have access to the application's service scope
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                await userManager.ConfirmEmailAsync(user, confirmationToken);
            }
        }
    }

    private async Task RegisterUser(HttpClient client, string email, string password)
    {
        var registrationInfo = new { Email = email, Password = password };
        var registrationResponse = await client.PostAsJsonAsync("/Authentication/register", registrationInfo);
        registrationResponse.EnsureSuccessStatusCode();
    }


    private class TokenResponse
    {
        public string? Token { get; set; }
    }
}
