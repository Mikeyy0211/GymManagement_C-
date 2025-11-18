using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Plans;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Plans;

public class PlanService
{
    private readonly IPlanRepository _repo;
    public PlanService(IPlanRepository repo) => _repo = repo;

    public async Task<PagedResult<PlanDto>> GetPagedAsync(PlanQuery q, CancellationToken ct)
    {
        q.Page = Math.Max(1, q.Page);
        q.PageSize = Math.Clamp(q.PageSize, 1, 100);

        var query = _repo.Query();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(s));
        }
        if (q.MinPrice.HasValue) query = query.Where(p => p.Price >= q.MinPrice.Value);
        if (q.MaxPrice.HasValue) query = query.Where(p => p.Price <= q.MaxPrice.Value);

        query = (q.SortBy?.ToLower(), q.SortDir?.ToLower()) switch
        {
            ("name", "asc") => query.OrderBy(p => p.Name),
            ("name", _)     => query.OrderByDescending(p => p.Name),
            ("price", "asc") => query.OrderBy(p => p.Price),
            ("price", _)     => query.OrderByDescending(p => p.Price),
            ("createdat", "asc") => query.OrderBy(p => p.CreatedAt),
            _                    => query.OrderByDescending(p => p.CreatedAt),
        };

        var total = await query.CountAsync(ct);
        var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize)
            .Select(p => new PlanDto {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationDays = p.DurationDays,
                MaxSessionsPerWeek = p.MaxSessionsPerWeek,
                RowVersionBase64 = p.RowVersion != null ? Convert.ToBase64String(p.RowVersion) : ""
            })
            .ToListAsync(ct);

        return new PagedResult<PlanDto> { Page = q.Page, PageSize = q.PageSize, TotalItems = total, Items = items };
    }

    public async Task<(PlanDto dto, string etag)> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken ct)
    {
        var plan = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted, ct)
                   ?? throw new KeyNotFoundException("Plan not found");

        var base64 = plan.RowVersion != null ? Convert.ToBase64String(plan.RowVersion) : "";
        var dto = new PlanDto {
            Id = plan.Id, Name = plan.Name, Price = plan.Price,
            DurationDays = plan.DurationDays, MaxSessionsPerWeek = plan.MaxSessionsPerWeek,
            RowVersionBase64 = base64
        };
        return (dto, base64);
    }

    public async Task<(PlanDto dto, string etag)> CreateAsync(CreatePlanRequest req, CancellationToken ct)
    {
        var plan = new MembershipPlan {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            Price = req.Price,
            DurationDays = req.DurationDays,
            MaxSessionsPerWeek = req.MaxSessionsPerWeek
        };
        await _repo.AddAsync(plan, ct);

        var base64 = plan.RowVersion != null ? Convert.ToBase64String(plan.RowVersion) : "";
        var dto = new PlanDto {
            Id = plan.Id, Name = plan.Name, Price = plan.Price,
            DurationDays = plan.DurationDays, MaxSessionsPerWeek = plan.MaxSessionsPerWeek,
            RowVersionBase64 = base64
        };
        return (dto, base64);
    }

    public async Task UpdateAsync(Guid id, UpdatePlanRequest req, string? ifMatchBase64, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted: false, ct)
                       ?? throw new KeyNotFoundException("Plan not found");

        // concurrency: ưu tiên If-Match header; fallback RowVersionBase64 trong body
        var src = ifMatchBase64 ?? req.RowVersionBase64;
        if (string.IsNullOrWhiteSpace(src))
            throw new DbUpdateConcurrencyException("Missing ETag (If-Match) or RowVersionBase64");

        var incoming = Convert.FromBase64String(src);
        if (existing.RowVersion is null || !incoming.SequenceEqual(existing.RowVersion))
            throw new DbUpdateConcurrencyException("The plan was modified by someone else. Please refresh and try again.");

        existing.Name = req.Name.Trim();
        existing.Price = req.Price;
        existing.DurationDays = req.DurationDays;
        existing.MaxSessionsPerWeek = req.MaxSessionsPerWeek;

        await _repo.UpdateAsync(existing, ct);
    }

    public async Task SoftDeleteAsync(Guid id, string? ifMatchBase64, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, asNoTracking: true, includeDeleted: true, ct)
                       ?? throw new KeyNotFoundException("Plan not found");

        if (existing.IsDeleted) return; // idempotent

        if (string.IsNullOrWhiteSpace(ifMatchBase64))
            throw new DbUpdateConcurrencyException("Missing ETag (If-Match)");

        var incoming = Convert.FromBase64String(ifMatchBase64);
        if (existing.RowVersion is null || !incoming.SequenceEqual(existing.RowVersion))
            throw new DbUpdateConcurrencyException("The plan was modified by someone else. Please refresh and try again.");

        await _repo.SoftDeleteAsync(existing, ct);
    }
}
