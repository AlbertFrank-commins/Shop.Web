using System;
using System.Threading.Tasks;
using Shop.Web.Entities.Payments;

namespace Shop.Web.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment?> GetByIdAsync(Guid id);
    }
}
