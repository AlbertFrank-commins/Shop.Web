using System;
using System.Threading.Tasks;
using Shop.Web.ViewModels.Payments;

namespace Shop.Web.Services.Payments
{
    public interface IPaymentService
    {
        Task<Guid> PayAsync(Guid userId, PaymentViewModel vm);
    }
}
