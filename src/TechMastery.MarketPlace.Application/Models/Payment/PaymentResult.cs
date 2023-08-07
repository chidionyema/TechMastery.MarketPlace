using System;
namespace TechMastery.MarketPlace.Application.Models.Payment
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string PaymentId { get; set; }
        public string ErrorMessage { get; set; }
        public string TransferId { get; set; }
    }
}