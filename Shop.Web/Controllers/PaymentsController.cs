using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Orders;
using Shop.Contracts.Payments;

namespace Shop.Web.Controllers;

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
        // Obtenemos la orden para saber el monto
        var order = await _ordersClient.GetByIdAsync(orderId);
        if (order is null)
        {
            return NotFound();
        }

        // Procesamos el pago "fake"
        var payment = await _paymentsClient.ProcessPaymentAsync(order.Id, order.TotalAmount);

        if (payment.Status == PaymentStatus.Succeeded)
        {
            // Marcamos la orden como Pagada
            await _ordersClient.UpdateStatusAsync(order.Id, OrderStatus.Paid);
        }
        else
        {
            // Podríamos guardar algún mensaje de error en TempData si quisiéramos
        }

        // Volvemos al detalle de la orden
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }
}
