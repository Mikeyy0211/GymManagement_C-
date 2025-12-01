namespace Gym.Application.DTOs.Payments;

public record PaymentDto(
    Guid Id,
    Guid MemberId,
    Guid PlanId,
    decimal Amount,
    DateTime PaidAt,
    DateTime ExpireAt
    );