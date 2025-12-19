using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Entities.Payments;

namespace Shop.Web.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ShopDbContext _db;

        public PaymentRepository(ShopDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Payment payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
        }

        public Task<Payment?> GetByIdAsync(Guid id)
            => _db.Payments.FirstOrDefaultAsync(x => x.Id == id);
    }
}

