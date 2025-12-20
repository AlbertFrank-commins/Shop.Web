using Shop.Contracts.Payments;

namespace Shop.Web.Clients;

public class InMemoryPaymentsClient : IPaymentsClient
{
    private static readonly List<PaymentDto> _payments = new();
    private static readonly object _lock = new();

    public Task<PaymentDto> ProcessPaymentAsync(
        Guid orderId,
        decimal amount,
        CancellationToken ct = default)
    {
        // Aquí puedes simular fallos si quieres, por ahora siempre "SUCCESS"
        var payment = new PaymentDto
        {
            Id = orderId,
            OrderId = orderId,
            Amount = amount,
            CreatedAt = DateTime.UtcNow,
            Status = PaymentStatus.Succeeded,
            Provider = "FakeProvider"
        };

        lock (_lock)
        {
            _payments.Add(payment);
        }

        return Task.FromResult(payment);
    }

    public Task<IReadOnlyList<PaymentDto>> GetPaymentsForOrderAsync(
        Guid orderId,
        CancellationToken ct = default)
    {
        lock (_lock)
        {
            var result = _payments
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList()
                .AsReadOnly();

            return Task.FromResult((IReadOnlyList<PaymentDto>)result);
        }
    }
}
