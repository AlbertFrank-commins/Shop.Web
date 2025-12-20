using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.Web.ViewModels.Payments
{
    public class PaymentCheckoutViewModel
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = "";
        public decimal Amount { get; set; }
        public string OrderStatus { get; set; } = "";

        [Required(ErrorMessage = "El titular es requerido.")]
        public string CardHolder { get; set; } = "";

        [Required(ErrorMessage = "El número de tarjeta es requerido.")]
        [MinLength(12, ErrorMessage = "Mínimo 12 dígitos.")]
        public string CardNumber { get; set; } = "";

        [Required(ErrorMessage = "La expiración es requerida.")]
        [Display(Name = "Expiración (MM/AA)")]
        public string Expiration { get; set; } = "";

        [Required(ErrorMessage = "El CVV es requerido.")]
        [MinLength(3, ErrorMessage = "Mínimo 3 dígitos.")]
        public string Cvv { get; set; } = "";

        public string? Error { get; set; }
    }
}
