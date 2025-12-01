using Gym.Application.DTOs.Payments;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Payments;

public class PaymentService
{
    private readonly IPaymentRepository _payments;
    private readonly IMemberRepository _members;
    private readonly IPlanRepository _plans;

    public PaymentService(IPaymentRepository payments, IMemberRepository members, IPlanRepository plans)
    {
        _payments = payments;
        _members = members;
        _plans = plans;
    }
    public async Task<PaymentDto> PayAsync(CreatePaymentRequest rq, CancellationToken ct)
    {
        var member = await _members.GetByIdAsync(rq.MemberId, false, false, ct)
                     ?? throw new KeyNotFoundException("Member not found");

        var plan = await _plans.GetByIdAsync(rq.PlanId, false, false, ct)
                   ?? throw new KeyNotFoundException("Plan not found");

        var payment = new Payment
        {
            MemberId = rq.MemberId,
            PlanId = rq.PlanId,
            Amount = plan.Price,
            PaidAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddDays(plan.DurationDays)
        };

        await _payments.AddAsync(payment, ct);

        // Update member plan
        member.MembershipPlanId = plan.Id;
        await _members.UpdateAsync(member, ct);

        return new PaymentDto(
            payment.Id,
            payment.MemberId,
            payment.PlanId,
            payment.Amount,
            payment.PaidAt,
            payment.ExpireAt
        );
    }

    public async Task<IEnumerable<PaymentDto>> GetByMemberAsync(Guid memberId, CancellationToken ct)
    {
        var list = await _payments.Query()
            .Where(x => !x.IsDeleted && x.MemberId == memberId)
            .ToListAsync(ct);

        return list.Select(p => new PaymentDto(
            p.Id, p.MemberId, p.PlanId, p.Amount, p.PaidAt, p.ExpireAt
        ));
    }

}