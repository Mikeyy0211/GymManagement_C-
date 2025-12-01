using Gym.Application.DTOs.Common;
using Gym.Application.DTOs.Trainers;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Trainers;

public class TrainerService
{
    private readonly IUnitOfWork _uow;

    public TrainerService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // =====================================================
    // 1. PAGED LIST
    // =====================================================
    public async Task<PagedResult<TrainerDto>> GetPagedAsync(TrainerQuery q, CancellationToken ct)
    {
        q.Page = Math.Max(1, q.Page);
        q.PageSize = Math.Clamp(q.PageSize, 1, 100);

        var query = _uow.Trainers.Query(); // ❗Không Include tại đây

        // SEARCH
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(t => t.User.FullName.ToLower().Contains(s));
        }

        // SORTING
        query = (q.SortBy?.ToLower(), q.SortDir?.ToLower()) switch
        {
            ("name", "asc")        => query.OrderBy(t => t.User.FullName),
            ("name", _)            => query.OrderByDescending(t => t.User.FullName),

            ("experience", "asc")  => query.OrderBy(t => t.ExperienceYears),
            ("experience", _)      => query.OrderByDescending(t => t.ExperienceYears),

            ("createdat", "asc")   => query.OrderBy(t => t.CreatedAt),
            _                      => query.OrderByDescending(t => t.CreatedAt)
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Include(t => t.User) // ❗Include AFTER sorting
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .Select(t => new TrainerDto
            {
                Id = t.Id,
                UserId = t.UserId,
                FullName = t.User.FullName,
                Specialty = t.Specialty,
                ExperienceYears = t.ExperienceYears,
                Phone = t.Phone,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<TrainerDto>
        {
            Page = q.Page,
            PageSize = q.PageSize,
            TotalItems = total,
            Items = items
        };
    }

    // =====================================================
    // 2. GET BY ID
    // =====================================================
    public async Task<TrainerDto> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken ct)
    {
        var trainer = await _uow.Trainers.Query(includeDeleted)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new KeyNotFoundException("Trainer not found");

        return new TrainerDto
        {
            Id = trainer.Id,
            UserId = trainer.UserId,
            FullName = trainer.User.FullName,
            Specialty = trainer.Specialty,
            ExperienceYears = trainer.ExperienceYears,
            Phone = trainer.Phone,
            CreatedAt = trainer.CreatedAt
        };
    }

    // =====================================================
    // 3. CREATE TRAINER
    // =====================================================
    public async Task<TrainerDto> CreateAsync(CreateTrainerRequest req, CancellationToken ct)
    {
        // Ensure user exists
        var user = await _uow.Users.GetByIdAsync(req.UserId, false, false, ct)
                ?? throw new KeyNotFoundException("User not found");

        var trainer = new TrainerProfile
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            Specialty = req.Specialty,
            ExperienceYears = req.ExperienceYears,
            Phone = req.Phone
        };

        await _uow.Trainers.AddAsync(trainer, ct);
        await _uow.SaveChangesAsync(ct);

        return new TrainerDto
        {
            Id = trainer.Id,
            UserId = trainer.UserId,
            FullName = user.FullName,
            Specialty = trainer.Specialty,
            ExperienceYears = trainer.ExperienceYears,
            Phone = trainer.Phone,
            CreatedAt = trainer.CreatedAt
        };
    }

    // =====================================================
    // 4. UPDATE TRAINER
    // =====================================================
    public async Task UpdateAsync(Guid id, UpdateTrainerRequest req, CancellationToken ct)
    {
        var trainer = await _uow.Trainers.GetByIdAsync(id, false, false, ct)
                      ?? throw new KeyNotFoundException("Trainer not found");

        trainer.Specialty = req.Specialty;
        trainer.ExperienceYears = req.ExperienceYears;
        trainer.Phone = req.Phone;

        await _uow.Trainers.UpdateAsync(trainer, ct);
    }

    // =====================================================
    // 5. SOFT DELETE
    // =====================================================
    public async Task SoftDeleteAsync(Guid id, CancellationToken ct)
    {
        var trainer = await _uow.Trainers.GetByIdAsync(id, false, true, ct)
                      ?? throw new KeyNotFoundException("Trainer not found");

        await _uow.Trainers.SoftDeleteAsync(trainer, ct);
    }
}