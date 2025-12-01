using System.Threading;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IMemberRepository Members { get; }
    IPlanRepository Plans { get; }
    ITrainerRepository Trainers { get; }
    IClassRepository Classes { get; }
    ISessionRepository Sessions { get; }
    IBookingRepository Bookings { get; }
    IAttendanceRepository Attendances { get; }
    IPaymentRepository Payments { get; }
    IReportRepository Reports { get; }

    // SAVE
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    // TRANSACTION SUPPORT
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}