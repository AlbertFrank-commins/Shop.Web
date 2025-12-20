using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Orders;
using Shop.Contracts.Payments;
using Shop.Web.ViewModels.Payments;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IPaymentsClient _paymentsClient;
        private readonly IOrdersClient _ordersClient;

        public PaymentsController(IPaymentsClient paymentsClient, IOrdersClient ordersClient)
        {
            _paymentsClient = paymentsClient;
            _ordersClient = ordersClient;
        }

        // GET: /Payments/Checkout?orderId=...
        [HttpGet]
        public async Task<IActionResult> Checkout(Guid orderId)
        {
            var order = await _ordersClient.GetByIdAsync(orderId);
            if (order is null) return NotFound();

            var vm = new PaymentCheckoutViewModel
            {
                OrderId = order.Id,
                OrderNumber = order.Id.ToString()[..8], // porque tu OrderDto no tiene OrderNumber
                Amount = order.TotalAmount,
                OrderStatus = order.Status.ToString(),
            };

            return View(vm);
        }

        // POST: /Payments/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(PaymentCheckoutViewModel vm)
        {
            var order = await _ordersClient.GetByIdAsync(vm.OrderId);
            if (order is null) return NotFound();

            // fuerza datos reales de la orden
            vm.OrderNumber = order.Id.ToString()[..8];
            vm.Amount = order.TotalAmount;
            vm.OrderStatus = order.Status.ToString();

            // Validación simple
            var digits = new string((vm.CardNumber ?? "").Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(vm.CardHolder))
            {
                vm.Error = "El titular es requerido.";
                return View(vm);
            }
            if (digits.Length < 12 || digits.Length > 19)
            {
                vm.Error = "Número de tarjeta inválido.";
                return View(vm);
            }

            // Expiración acepta MM/AA o MMYY (0230)
            if (!IsValidExpiration(vm.Expiration))
            {
                vm.Error = "Expiración inválida. Usa MM/AA.";
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Cvv) || vm.Cvv.Length < 3 || vm.Cvv.Length > 4 || !vm.Cvv.All(char.IsDigit))
            {
                vm.Error = "CVV inválido. Debe tener 3 o 4 dígitos.";
                return View(vm);
            }

            var last4 = digits.Length >= 4 ? digits[^4..] : "0000";

            // ✅ Aquí usamos TU InMemoryPaymentsClient
            var payment = await _paymentsClient.ProcessPaymentAsync(order.Id, order.TotalAmount);

            if (payment.Status == PaymentStatus.Succeeded)
            {
                await _ordersClient.UpdateStatusAsync(order.Id, OrderStatus.Paid);

                // Pasamos data a Success por querystring (sin TempData)
                return RedirectToAction(nameof(Success), new
                {
                    paymentId = Guid.NewGuid(), // no lo tienes en PaymentDto, generamos uno fake para UI
                    amount = order.TotalAmount,
                    last4,
                    holder = vm.CardHolder
                });
            }

            vm.Error = "No se pudo procesar el pago. Intenta nuevamente.";
            return View(vm);
        }

        // GET: /Payments/Success?... (sin DB)
        [HttpGet]
        public IActionResult Success(Guid paymentId, decimal amount, string last4, string holder)
        {
            var model = new PaymentSuccessViewModel
            {
                PaymentId = paymentId,
                Amount = amount,
                Last4 = last4 ?? "0000",
                CardHolder = holder ?? "Cliente"
            };

            return View(model);
        }

        private bool IsValidExpiration(string? expiration)
        {
            if (string.IsNullOrWhiteSpace(expiration)) return false;

            expiration = expiration.Trim();

            // acepta "0230" -> "02/30"
            if (expiration.Length == 4 && expiration.All(char.IsDigit))
                expiration = expiration.Insert(2, "/");

            var parts = expiration.Split('/');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out var mm)) return false;
            if (mm < 1 || mm > 12) return false;

            if (!int.TryParse(parts[1], out var yy)) return false;
            var year = 2000 + yy;

            var expDate = new DateTime(year, mm, DateTime.DaysInMonth(year, mm), 23, 59, 59, DateTimeKind.Utc);
            return expDate >= DateTime.UtcNow;
        }
    }
}
