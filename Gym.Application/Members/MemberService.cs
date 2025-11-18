using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Members;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Members;

public class MemberService
{
    private readonly IMemberRepository _repo;
    private readonly IPlanRepository _plans;

    public MemberService(IMemberRepository repo, IPlanRepository plans)
    {
        _repo = repo;
        _plans = plans;
    }

    public async Task<PagedResult<MemberDto>> GetPagedAsync(MemberQuery q, CancellationToken ct)
    {
        q.Page = Math.Max(1, q.Page);
        q.PageSize = Math.Clamp(q.PageSize, 1, 100);

        var query = _repo.Query();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(m => m.FullName.ToLower().Contains(s));
        }

        if (q.PlanId.HasValue)
            query = query.Where(m => m.MembershipPlanId == q.PlanId.Value);

        if (q.DobFrom.HasValue)
            query = query.Where(m => m.DateOfBirth >= q.DobFrom.Value);

        if (q.DobTo.HasValue)
            query = query.Where(m => m.DateOfBirth <= q.DobTo.Value);

        query = (q.SortBy?.ToLower(), q.SortDir?.ToLower()) switch
        {
            ("fullname", "asc") => query.OrderBy(m => m.FullName),
            ("fullname", _) => query.OrderByDescending(m => m.FullName),
            ("dateofbirth", "asc") => query.OrderBy(m => m.DateOfBirth),
            ("dateofbirth", _) => query.OrderByDescending(m => m.DateOfBirth),
            ("createdat", "asc") => query.OrderBy(m => m.CreatedAt),
            _ => query.OrderByDescending(m => m.CreatedAt),
        };

        var total = await query.CountAsync(ct);

        var items = await query.Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(m => new MemberDto
            {
                Id = m.Id,
                FullName = m.FullName,
                DateOfBirth = m.DateOfBirth,
                MembershipPlanId = m.MembershipPlanId,
                MembershipPlanName = m.MembershipPlan != null ? m.MembershipPlan.Name : null,
                RowVersionBase64 = Convert.ToBase64String(m.RowVersion ?? Array.Empty<byte>())
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

    public async Task<(MemberDto dto, string etag)> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken ct)
    {
        var m = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted, ct)
                ?? throw new KeyNotFoundException("Member not found");

        var base64 = Convert.ToBase64String(m.RowVersion ?? Array.Empty<byte>());

        var dto = new MemberDto
        {
            Id = m.Id,
            FullName = m.FullName,
            DateOfBirth = m.DateOfBirth,
            MembershipPlanId = m.MembershipPlanId,
            MembershipPlanName = m.MembershipPlan?.Name,
            RowVersionBase64 = base64
        };

        return (dto, base64);
    }

    public async Task<(MemberDto dto, string etag)> CreateAsync(CreateMemberRequest req, CancellationToken ct)
    {
        if (req.MembershipPlanId.HasValue)
        {
            var plan = await _plans.GetByIdAsync(req.MembershipPlanId.Value, true, false, ct);
            if (plan == null) throw new KeyNotFoundException("Membership plan not found");
        }

        var member = new Member
        {
            FullName = req.FullName,
            DateOfBirth = req.DateOfBirth!.Value,   
            MembershipPlanId = req.MembershipPlanId
        };

        await _repo.AddAsync(member, ct);

        var base64 = Convert.ToBase64String(member.RowVersion ?? Array.Empty<byte>());

        var dto = new MemberDto
        {
            Id = member.Id,
            FullName = member.FullName,
            DateOfBirth = member.DateOfBirth,
            MembershipPlanId = member.MembershipPlanId,
            MembershipPlanName = null,
            RowVersionBase64 = base64
        };

        return (dto, base64);
    }

    public async Task UpdateAsync(Guid id, UpdateMemberRequest req, string? ifMatchBase64, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted: false, ct)
                       ?? throw new KeyNotFoundException("Member not found");

        var src = ifMatchBase64 ?? req.RowVersionBase64;

        if (string.IsNullOrWhiteSpace(src))
            throw new DbUpdateConcurrencyException("Missing ETag (If-Match) or RowVersionBase64");

        var incoming = Convert.FromBase64String(src);

        if (existing.RowVersion == null || !incoming.SequenceEqual(existing.RowVersion))
            throw new DbUpdateConcurrencyException("The member was modified by someone else.");

        // UPDATE
        existing.FullName = req.FullName.Trim();
        existing.DateOfBirth = req.DateOfBirth!.Value;
        existing.MembershipPlanId = req.MembershipPlanId;

        await _repo.UpdateAsync(existing, ct);
    }

    public async Task SoftDeleteAsync(Guid id, string? ifMatchBase64, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted: true, ct)
                       ?? throw new KeyNotFoundException("Member not found");

        if (existing.IsDeleted) return;

        if (string.IsNullOrWhiteSpace(ifMatchBase64))
            throw new DbUpdateConcurrencyException("Missing ETag (If-Match)");

        var incoming = Convert.FromBase64String(ifMatchBase64);

        if (existing.RowVersion == null || !incoming.SequenceEqual(existing.RowVersion))
            throw new DbUpdateConcurrencyException("The member was modified by someone else.");

        await _repo.SoftDeleteAsync(existing, ct);
    }

    public async Task AssignPlanAsync(Guid id, AssignPlanRequest request, byte[] incomingRowVersion, CancellationToken ct)
    {
        var member = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted: false, ct)
                      ?? throw new KeyNotFoundException("Member not found");

        // Check concurrency
        if (member.RowVersion == null || !member.RowVersion.SequenceEqual(incomingRowVersion))
            throw new DbUpdateConcurrencyException("Concurrency conflict occurred");

        member.MembershipPlanId = request.MembershipPlanId;

        await _repo.UpdateAsync(member, ct);
    }
}
