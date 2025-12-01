using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gym.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly GymDbContext _db;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        GymDbContext db,
        IUserRepository users,
        IMemberRepository members,
        IPlanRepository plans,
        ITrainerRepository trainers,
        IClassRepository classes,
        ISessionRepository sessions,
        IBookingRepository bookings,
        IAttendanceRepository attendances,
        IPaymentRepository payments,
        IReportRepository reports)
    {
        _db = db;
        Users = users;
        Members = members;
        Plans = plans;
        Trainers = trainers;
        Classes = classes;
        Sessions = sessions;
        Bookings = bookings;
        Attendances = attendances;
        Payments = payments;
        Reports = reports;
    }

    public IUserRepository Users { get; }
    public IMemberRepository Members { get; }
    public IPlanRepository Plans { get; }
    public ITrainerRepository Trainers { get; }
    public IClassRepository Classes { get; }
    public ISessionRepository Sessions { get; }
    public IBookingRepository Bookings { get; }
    public IAttendanceRepository Attendances { get; }
    public IPaymentRepository Payments { get; }
    public IReportRepository Reports { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null) return;
        _transaction = await _db.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;

        await _db.SaveChangesAsync(ct);
        await _transaction!.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;

        await _transaction!.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
            await _transaction.DisposeAsync();
    }
}