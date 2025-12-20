using System;

namespace Shop.Web.ViewModels.Payments
{
    public class PaymentSuccessViewModel
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }

        public string Last4 { get; set; } = "";
        public string CardHolder { get; set; } = "";
    }
}
