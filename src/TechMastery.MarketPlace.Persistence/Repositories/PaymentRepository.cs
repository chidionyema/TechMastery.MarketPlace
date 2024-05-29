

using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Persistence.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public Task<Payment> GetSuccessfulPaymentByOrderId(Guid orderId)
        {
            throw new NotImplementedException();
        }
    }
}