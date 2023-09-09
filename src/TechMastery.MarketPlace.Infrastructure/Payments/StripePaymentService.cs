using Microsoft.Extensions.Logging;
using Stripe;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Models.Payment;

namespace TechMastery.MarketPlace.Infrastructure.Payment
{
    public class StripePaymentService : IPaymentService
    {
        private readonly string _stripeSecretKey;
        private readonly ILogger<StripePaymentService> _logger;
        private const int MAX_RETRIES = 3;

        public StripePaymentService(string stripeSecretKey, ILogger<StripePaymentService> logger)
        {
            if (!stripeSecretKey.StartsWith("sk_live_") && !stripeSecretKey.StartsWith("sk_test_"))
                throw new ArgumentException("Invalid StripeSecretKey format.", nameof(stripeSecretKey));

            _stripeSecretKey = stripeSecretKey;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
        {
            try
            {
                var charge = await CreateChargeAsync(paymentInfo, cancellationToken);
                var transfer = await CreateTransferAsync(paymentInfo, cancellationToken);

                LogSuccessfulPayment(charge.Id, transfer.Id);

                return new PaymentResult
                {
                    IsSuccess = true,
                    PaymentId = charge.Id,
                    TransferId = transfer.Id,
                };
            }
            catch (StripeException stripeException)
            {
                _logger.LogError(stripeException, "Stripe payment processing failed.");
                return HandleStripeException(stripeException);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during payment processing.");
                throw;
            }
        }

        private async Task<Charge> CreateChargeAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
        {
            var chargeOptions = BuildChargeOptions(paymentInfo);
            var requestOptions = new RequestOptions { IdempotencyKey = Guid.NewGuid().ToString() };

            var service = new ChargeService();
            int retryCount = 0;
            while (retryCount < MAX_RETRIES)
            {
                try
                {
                    return await service.CreateAsync(chargeOptions, requestOptions, cancellationToken);
                }
                catch (HttpRequestException)
                {
                    retryCount++;
                    await Task.Delay(2000); // wait for 2 seconds before retrying
                }
            }
            throw new Exception("Max retry attempts reached for CreateChargeAsync.");
        }

        private async Task<Transfer> CreateTransferAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
        {
            var transferOptions = BuildTransferOptions(paymentInfo);
            var requestOptions = new RequestOptions { IdempotencyKey = Guid.NewGuid().ToString() };

            var transferService = new TransferService();
            int retryCount = 0;
            while (retryCount < MAX_RETRIES)
            {
                try
                {
                    return await transferService.CreateAsync(transferOptions, requestOptions, cancellationToken);
                }
                catch (HttpRequestException)
                {
                    retryCount++;
                    await Task.Delay(2000); // wait for 2 seconds before retrying
                }
            }
            throw new Exception("Max retry attempts reached for CreateTransferAsync.");
        }

        private ChargeCreateOptions BuildChargeOptions(PaymentInfo paymentInfo)
        {
            return new ChargeCreateOptions
            {
                Amount = (long)paymentInfo.Amount,
                Currency = paymentInfo.Currency,
                Description = paymentInfo.Description,
                Source = paymentInfo.Token,
            };
        }

        private TransferCreateOptions BuildTransferOptions(PaymentInfo paymentInfo)
        {
            var transferAmount = paymentInfo.GetPaymentAmount();

            return new TransferCreateOptions
            {
                Amount = (long)transferAmount,
                Currency = paymentInfo.Currency,
                Destination = paymentInfo.SellerStripeAccountId,
            };
        }

        private static PaymentResult HandleStripeException(StripeException stripeException)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorMessage = stripeException.Message,
            };
        }

        private void LogSuccessfulPayment(string chargeId, string transferId)
        {
            _logger.LogInformation("Payment processed successfully. Charge ID: {ChargeId}, Transfer ID: {TransferId}", chargeId, transferId);
        }
    }
}
