using System.ComponentModel.DataAnnotations;

namespace Shop.Web.ViewModels.Payments
{
    public class PaymentViewModel
    {
        [Required]
        [Range(1, 999999)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Nombre del titular")]
        public string CardHolder { get; set; } = "";

        [Required]
        [Display(Name = "Número de tarjeta")]
        [MinLength(12)]
        public string CardNumber { get; set; } = "";

        [Required]
        [Display(Name = "Expiración (MM/AA)")]
        public string Expiration { get; set; } = "";

        [Required]
        [Display(Name = "CVV")]
        [MinLength(3)]
        public string Cvv { get; set; } = "";
    }
}

