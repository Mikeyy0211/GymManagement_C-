using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Members;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Members;

public class MemberService
{
    private readonly IUnitOfWork _uow;

    public MemberService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // ==================================================
    // 1. GET PAGED MEMBERS
    // ==================================================
    public async Task<PagedResult<MemberDto>> GetPagedAsync(MemberQuery q, CancellationToken ct)
    {
        q.Page = Math.Max(1, q.Page);
        q.PageSize = Math.Clamp(q.PageSize, 1, 100);

        var query = _uow.Members.Query();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(m => m.FullName.ToLower().Contains(s));
        }

        query = q.SortBy?.ToLower() switch
        {
            "name" => q.SortDir == "asc" ? query.OrderBy(m => m.FullName) : query.OrderByDescending(m => m.FullName),
            "createdat" => q.SortDir == "asc" ? query.OrderBy(m => m.CreatedAt) : query.OrderByDescending(m => m.CreatedAt),
            _ => query.OrderByDescending(m => m.CreatedAt)
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(m => new MemberDto
            {
                Id = m.Id,
                FullName = m.FullName,
                DateOfBirth = m.DateOfBirth,
                MembershipPlanId = m.MembershipPlanId,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<MemberDto>
        {
            Page = q.Page,
            PageSize = q.PageSize,
            TotalItems = total,
            Items = items
        };
    }

    // ==================================================
    // 2. GET BY ID
    // ==================================================
    public async Task<MemberDto> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken ct)
    {
        var member = await _uow.Members.GetByIdAsync(id, true, includeDeleted, ct)
                      ?? throw new KeyNotFoundException("Member not found");

        return new MemberDto
        {
            Id = member.Id,
            FullName = member.FullName,
            DateOfBirth = member.DateOfBirth,
            MembershipPlanId = member.MembershipPlanId,
            CreatedAt = member.CreatedAt
        };
    }

    // ==================================================
    // 3. CREATE
    // ==================================================
    public async Task<MemberDto> CreateAsync(CreateMemberRequest req, CancellationToken ct)
    {
        var member = new Member
        {
            Id = Guid.NewGuid(),
            FullName = req.FullName.Trim(),
            DateOfBirth = req.DateOfBirth,
            MembershipPlanId = req.MembershipPlanId
        };

        await _uow.Members.AddAsync(member, ct);
        await _uow.SaveChangesAsync(ct);

        return new MemberDto
        {
            Id = member.Id,
            FullName = member.FullName,
            DateOfBirth = member.DateOfBirth,
            MembershipPlanId = member.MembershipPlanId,
            CreatedAt = member.CreatedAt
        };
    }

    // ==================================================
    // 4. UPDATE
    // ==================================================
    public async Task UpdateAsync(Guid id, UpdateMemberRequest req, CancellationToken ct)
    {
        var member = await _uow.Members.GetByIdAsync(id, false, false, ct)
                      ?? throw new KeyNotFoundException("Member not found");

        member.FullName = req.FullName.Trim();
        member.DateOfBirth = req.DateOfBirth;

        await _uow.Members.UpdateAsync(member, ct);
    }

    // ==================================================
    // 5. SOFT DELETE
    // ==================================================
    public async Task SoftDeleteAsync(Guid id, CancellationToken ct)
    {
        var member = await _uow.Members.GetByIdAsync(id, false, true, ct)
                      ?? throw new KeyNotFoundException("Member not found");

        await _uow.Members.SoftDeleteAsync(member, ct);
    }

    // ==================================================
    // 6. ASSIGN MEMBERSHIP PLAN (transaction)
    // ==================================================
    public async Task AssignPlanAsync(Guid memberId, Guid planId, CancellationToken ct)
    {
        var member = await _uow.Members.GetByIdAsync(memberId, false, false, ct)
                      ?? throw new KeyNotFoundException("Member not found");

        var plan = await _uow.Plans.GetByIdAsync(planId, true, false, ct)
                    ?? throw new KeyNotFoundException("Plan not found");

        await _uow.BeginTransactionAsync(ct);

        try
        {
            member.MembershipPlanId = planId;
            await _uow.Members.UpdateAsync(member, ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }
}