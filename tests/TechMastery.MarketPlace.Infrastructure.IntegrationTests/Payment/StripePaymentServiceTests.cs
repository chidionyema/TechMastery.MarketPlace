using Stripe;
using TechMastery.MarketPlace.Application.Models.Payment;

namespace TechMastery.MarketPlace.Infrastructure.Tests
{
    [Collection("StripeTestCollection")] // Define your Stripe test collection
    public class StripePaymentServiceTests
    {
        private readonly StripeTestContext _stripeTestContext;

        public StripePaymentServiceTests(StripeTestContext stripeTestContext)
        {
            _stripeTestContext = stripeTestContext;
        }

        [Fact]
        public async Task ProcessPaymentAsync_SuccessfulPayment_ReturnsSuccessfulResult()
        {
            // Arrange
            var stripeSecretKey = _stripeTestContext.StripeSecretKey;
            var tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Number = "4242424242424242", // Use a valid test card number
                    ExpMonth = "12",
                    ExpYear = (DateTime.Now.Year + 1) .ToString(),
                    Cvc = "123"
                }
            };

            var service = new TokenService(new StripeClient(stripeSecretKey));
            var stripeToken = service.Create(tokenOptions);
            var paymentInfo = new PaymentInfo
            {
                Token = stripeToken.Id,
                Amount = 100.00m,
                SellerCut = 85.00m,
                Currency = "usd",
                Description = "Sample payment",
               // SellerStripeAccountId = "seller_stripe_account_id"
            };

            var stripePaymentService = new StripePaymentService(stripeSecretKey);

            // Act
            var result = await stripePaymentService.ProcessPaymentAsync(paymentInfo, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.PaymentId);
            Assert.NotNull(result.TransferId);
        }

        [Fact]
        public async Task ProcessPaymentAsync_StripeException_ReturnsErrorResult()
        {
            // Arrange
            var stripeSecretKey = _stripeTestContext.StripeSecretKey;
            var paymentInfo = new PaymentInfo
            {
                Token = "invalid_token",
                Amount = 50.00m,
                SellerCut = 40.00m,
                Currency = "usd",
                Description = "Sample payment",
                SellerStripeAccountId = "seller_stripe_account_id"
            };

            // Simulate a scenario where Stripe API throws an exception
            StripeConfiguration.ApiKey = stripeSecretKey;
            var service = new ChargeService();
            var chargeOptions = new ChargeCreateOptions
            {
                Amount = 500,
                Currency = "usd",
                Source = "tok_chargeDeclined",
            };
            await Assert.ThrowsAsync<StripeException>(async () => await service.CreateAsync(chargeOptions));

            var stripePaymentService = new StripePaymentService(stripeSecretKey);

            // Act
            var result = await stripePaymentService.ProcessPaymentAsync(paymentInfo, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
