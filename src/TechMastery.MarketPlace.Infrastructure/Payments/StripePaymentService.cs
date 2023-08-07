using Stripe;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Models.Payment;

public class StripePaymentService : IPaymentService
{
    private readonly string _stripeSecretKey;

    public StripePaymentService(string stripeSecretKey)
    {
        if (string.IsNullOrWhiteSpace(stripeSecretKey))
        {
            throw new ArgumentException("StripeSecretKey is missing or empty.", nameof(stripeSecretKey));
        }

        _stripeSecretKey = stripeSecretKey;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
    {
        StripeConfiguration.ApiKey = _stripeSecretKey;

        try
        {
            var charge = await CreateChargeAsync(paymentInfo, cancellationToken);
            var transfer = await CreateTransferAsync(paymentInfo, cancellationToken);

            return new PaymentResult
            {
                IsSuccess = true,
                PaymentId = charge.Id,
                TransferId = transfer.Id,
            };
        }
        catch (StripeException stripeException)
        {
            return HandleStripeException(stripeException);
        }
    }

    private async Task<Charge> CreateChargeAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
    {
        var chargeOptions = BuildChargeOptions(paymentInfo);

        var service = new ChargeService();
        return await service.CreateAsync(chargeOptions,null, cancellationToken);
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
        // validate seller cut of payment
        if (transferAmount != paymentInfo.SellerCut)
        {
            throw new InvalidOperationException();
        }

        return new TransferCreateOptions
        {
            Amount = (long) transferAmount,
            Currency = paymentInfo.Currency,
            Destination = paymentInfo.SellerStripeAccountId,
        };
    }

    private static long CalculateFeeAmount(decimal amount)
    {
        const decimal feePercentage = 0.15m;
        return (long)(amount * feePercentage);
    }

    private static PaymentResult HandleStripeException(StripeException stripeException)
    {
        return new PaymentResult
        {
            IsSuccess = false,
            ErrorMessage = stripeException.Message,
        };
    }
}
