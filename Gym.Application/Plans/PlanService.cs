using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Plans;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Plans;

public class PlanService
{
    private readonly IUnitOfWork _uow;

    public PlanService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // ===============================================================
    // 1. GET PAGED
    // ===============================================================
    public async Task<PagedResult<PlanDto>> GetPagedAsync(PlanQuery q, CancellationToken ct)
    {
        q.Page = Math.Max(1, q.Page);
        q.PageSize = Math.Clamp(q.PageSize, 1, 100);

        var query = _uow.Plans.Query();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(s));
        }

        if (q.MinPrice.HasValue) query = query.Where(p => p.Price >= q.MinPrice.Value);
        if (q.MaxPrice.HasValue) query = query.Where(p => p.Price <= q.MaxPrice.Value);

        query = (q.SortBy?.ToLower(), q.SortDir?.ToLower()) switch
        {
            ("name", "asc")        => query.OrderBy(p => p.Name),
            ("name", _)            => query.OrderByDescending(p => p.Name),
            ("price", "asc")       => query.OrderBy(p => p.Price),
            ("price", _)           => query.OrderByDescending(p => p.Price),
            ("createdat", "asc")   => query.OrderBy(p => p.CreatedAt),
            _                      => query.OrderByDescending(p => p.CreatedAt),
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(p => new PlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationDays = p.DurationDays,
                MaxSessionsPerWeek = p.MaxSessionsPerWeek,
                RowVersionBase64 = p.RowVersion != null ? Convert.ToBase64String(p.RowVersion) : ""
            })
            .ToListAsync(ct);

        return new PagedResult<PlanDto>
        {
            Page = q.Page,
            PageSize = q.PageSize,
            TotalItems = total,
            Items = items
        };
    }

    // ===============================================================
    // 2. GET BY ID
    // ===============================================================
    public async Task<(PlanDto dto, string etag)> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken ct)
    {
        var plan = await _uow.Plans.GetByIdAsync(id, asNoTracking: true, includeDeleted, ct)
                   ?? throw new KeyNotFoundException("Plan not found");

        var etag = plan.RowVersion != null ? Convert.ToBase64String(plan.RowVersion) : "";

        var dto = new PlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Price = plan.Price,
            DurationDays = plan.DurationDays,
            MaxSessionsPerWeek = plan.MaxSessionsPerWeek,
            RowVersionBase64 = etag
        };

        return (dto, etag);
    }

    // ===============================================================
    // 3. CREATE
    // ===============================================================
    public async Task<(PlanDto dto, string etag)> CreateAsync(CreatePlanRequest req, CancellationToken ct)
    {
        var plan = new MembershipPlan
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Price = req.Price,
            DurationDays = req.DurationDays,
            MaxSessionsPerWeek = req.MaxSessionsPerWeek
        };

        await _uow.Plans.AddAsync(plan, ct);
        await _uow.SaveChangesAsync(ct);

        var etag = plan.RowVersion != null ? Convert.ToBase64String(plan.RowVersion) : "";

        return (
            new PlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                MaxSessionsPerWeek = plan.MaxSessionsPerWeek,
                RowVersionBase64 = etag
            },
            etag
        );
    }

    // ===============================================================
    // 4. UPDATE
    // ===============================================================
    public async Task UpdateAsync(Guid id, UpdatePlanRequest req, string? etagHeader, CancellationToken ct)
    {
        var plan = await _uow.Plans.GetByIdAsync(id, true, false, ct)
                   ?? throw new KeyNotFoundException("Plan not found");

        var etag = etagHeader ?? req.RowVersionBase64;

        if (string.IsNullOrEmpty(etag))
            throw new DbUpdateConcurrencyException("Missing ETag");

        var incoming = Convert.FromBase64String(etag);

        if (plan.RowVersion is null || !incoming.SequenceEqual(plan.RowVersion))
            throw new DbUpdateConcurrencyException("Plan was modified by someone else");

        plan.Name = req.Name.Trim();
        plan.Price = req.Price;
        plan.DurationDays = req.DurationDays;
        plan.MaxSessionsPerWeek = req.MaxSessionsPerWeek;

        await _uow.Plans.UpdateAsync(plan, ct);
    }

    // ===============================================================
    // 5. SOFT DELETE
    // ===============================================================
    public async Task SoftDeleteAsync(Guid id, string etag, CancellationToken ct)
    {
        var plan = await _uow.Plans.GetByIdAsync(id, true, true, ct)
                   ?? throw new KeyNotFoundException("Plan not found");

        if (plan.RowVersion == null || !Convert.FromBase64String(etag).SequenceEqual(plan.RowVersion))
            throw new DbUpdateConcurrencyException("Plan was modified");

        await _uow.Plans.SoftDeleteAsync(plan, ct);
    }
}