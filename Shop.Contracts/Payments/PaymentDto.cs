using System;

namespace Shop.Contracts.Payments;

public class PaymentDto
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid OrderId { get; init; }

    public decimal Amount { get; init; }

    public DateTime CreatedAt { get; init; }

    public PaymentStatus Status { get; init; }

    public string Provider { get; init; } = "FakeProvider";

    public string? FailureReason { get; init; }
}
