using System;
using System.Linq;
using System.Threading.Tasks;
using Shop.Web.Entities.Payments;
using Shop.Web.Repositories.Payments;
using Shop.Web.ViewModels.Payments;

namespace Shop.Web.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;

        public PaymentService(IPaymentRepository repo)
        {
            _repo = repo;
        }

        public async Task<Guid> PayAsync(Guid userId, PaymentViewModel vm)
        {
            // ⚠️ Pago SIMULADO (monolito académico)
            var last4 = new string(vm.CardNumber.Where(char.IsDigit).ToArray());
            last4 = last4.Length >= 4 ? last4[^4..] : "0000";

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = vm.Amount,
                Currency = "USD",
                Status = "PAID",
                CardHolder = vm.CardHolder,
                Last4 = last4,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(payment);
            return payment.Id;
        }
    }
}

