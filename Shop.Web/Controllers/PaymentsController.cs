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
    [Authorize] // si no quieres obligar login, quítalo
    public class PaymentsController : Controller
    {
        private readonly IPaymentsClient _paymentsClient;
        private readonly IOrdersClient _ordersClient;

        public PaymentsController(IPaymentsClient paymentsClient, IOrdersClient ordersClient)
        {
            _paymentsClient = paymentsClient;
            _ordersClient = ordersClient;
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(Guid orderId)
        {
            var order = await _ordersClient.GetByIdAsync(orderId);
            if (order is null) return NotFound();

            var payment = await _paymentsClient.ProcessPaymentAsync(order.Id, order.TotalAmount);

            if (payment.Status == PaymentStatus.Succeeded)
            {
                await _ordersClient.UpdateStatusAsync(order.Id, OrderStatus.Paid);

                // opcional: mandar a pantalla bonita de "Pago realizado"
                return RedirectToAction(nameof(Success), new { orderId = order.Id });
            }

            TempData["PaymentError"] = "No se pudo procesar el pago. Intenta nuevamente.";
            return RedirectToAction("Details", "Orders", new { id = order.Id });
        }

       
        [HttpGet]
        public async Task<IActionResult> Checkout(Guid orderId)
        {
            var order = await _ordersClient.GetByIdAsync(orderId);
            if (order is null) return NotFound();

            var vm = new PaymentViewModel
            {
                Amount = order.TotalAmount
            };

            ViewBag.OrderId = orderId;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Guid orderId, PaymentViewModel vm)
        {
            var order = await _ordersClient.GetByIdAsync(orderId);
            if (order is null) return NotFound();

            // forzamos el monto real de la orden (evita que editen el input)
            vm.Amount = order.TotalAmount;

            if (!ModelState.IsValid)
            {
                ViewBag.OrderId = orderId;
                return View(vm);
            }

            // Simulación de pago: guardamos "last4" solo para mostrar
            var digits = new string((vm.CardNumber ?? "").Where(char.IsDigit).ToArray());
            var last4 = digits.Length >= 4 ? digits.Substring(digits.Length - 4) : "0000";

            // Procesa el pago usando tu PaymentsClient actual (fake)
            var payment = await _paymentsClient.ProcessPaymentAsync(order.Id, order.TotalAmount);

            if (payment.Status == PaymentStatus.Succeeded)
            {
                await _ordersClient.UpdateStatusAsync(order.Id, OrderStatus.Paid);

                // Mandamos data para mostrar en Success
                TempData["Paid_OrderId"] = order.Id.ToString();
                TempData["Paid_Amount"] = order.TotalAmount.ToString();
                TempData["Paid_Last4"] = last4;
                TempData["Paid_Holder"] = vm.CardHolder ?? "";

                return RedirectToAction(nameof(Success));
            }

            ModelState.AddModelError("", "No se pudo procesar el pago. Intenta nuevamente.");
            ViewBag.OrderId = orderId;
            return View(vm);
        }

      
        [HttpGet]
        public IActionResult Success(Guid? orderId = null)
        {
            // Si vienes por Pay(orderId) y no usaste TempData, puedes pasar orderId
            if (orderId.HasValue)
            {
                var modelFromOrder = new PaymentSuccessViewModel
                {
                    PaymentId = Guid.NewGuid(), // fake
                    Amount = 0,                 // si quieres: buscar orden y poner total
                    Last4 = "0000",
                    CardHolder = "Cliente"
                };
                return View(modelFromOrder);
            }

            // Si vienes por Checkout POST, usamos TempData
            var model = new PaymentSuccessViewModel
            {
                PaymentId = Guid.NewGuid(),
                Amount = decimal.TryParse((string?)TempData["Paid_Amount"], out var amt) ? amt : 0,
                Last4 = (string?)TempData["Paid_Last4"] ?? "0000",
                CardHolder = (string?)TempData["Paid_Holder"] ?? "Cliente"
            };

            return View(model);
        }
    }
}

    

