using System;
using System.Threading;
using System.Threading.Tasks;
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

        public StripePaymentService(string stripeSecretKey, ILogger<StripePaymentService> logger)
        {
            _stripeSecretKey = stripeSecretKey ?? throw new ArgumentNullException(nameof(stripeSecretKey), "StripeSecretKey is missing or null.");
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
        }

        private async Task<Charge> CreateChargeAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
        {
            var chargeOptions = BuildChargeOptions(paymentInfo);

            var service = new ChargeService();
            return await service.CreateAsync(chargeOptions, null, cancellationToken);
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

        private async Task<Transfer> CreateTransferAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
        {
            var transferOptions = BuildTransferOptions(paymentInfo);

            var transferService = new TransferService();
            return await transferService.CreateAsync(transferOptions, null, cancellationToken);
        }

        private TransferCreateOptions BuildTransferOptions(PaymentInfo paymentInfo)
        {
            var feeAmount = CalculateFeeAmount(paymentInfo.Amount);
            var transferAmount = paymentInfo.Amount - feeAmount;

            ValidateSellerCut(paymentInfo.SellerCut, transferAmount);

            return new TransferCreateOptions
            {
                Amount = (long)transferAmount,
                Currency = paymentInfo.Currency,
                Destination = paymentInfo.SellerStripeAccountId,
            };
        }

        private static long CalculateFeeAmount(decimal amount)
        {
            const decimal feePercentage = 0.15m;
            return (long)(amount * feePercentage);
        }

        private static void ValidateSellerCut(decimal sellerCut, decimal transferAmount)
        {
            if (sellerCut != transferAmount)
            {
                throw new InvalidOperationException("Seller cut does not match transfer amount.");
            }
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
