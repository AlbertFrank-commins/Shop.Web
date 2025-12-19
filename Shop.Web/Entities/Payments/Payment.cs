using System;

namespace Shop.Web.Entities.Payments
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }      
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";

        public string Status { get; set; } = "PAID"; 
        public string Last4 { get; set; } = "";
        public string CardHolder { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

