using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Contracts.Payments;

public interface IPaymentsClient
{
    Task<PaymentDto> ProcessPaymentAsync(
        Guid orderId,
        decimal amount,
        CancellationToken ct = default);

    Task<IReadOnlyList<PaymentDto>> GetPaymentsForOrderAsync(
        Guid orderId,
        CancellationToken ct = default);
}
