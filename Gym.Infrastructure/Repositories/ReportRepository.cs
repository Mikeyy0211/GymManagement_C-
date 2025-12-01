using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;

namespace Gym.Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly GymDbContext _db;

    public ReportRepository(GymDbContext db)
    {
        _db = db;
    }

    public IQueryable<Payment> PaymentsQuery() => _db.Payments;
    public IQueryable<Booking> BookingsQuery() => _db.Bookings;
    public IQueryable<ClassSession> SessionsQuery() => _db.ClassSessions;
    public IQueryable<GymClass> ClassesQuery() => _db.GymClasses;
    public IQueryable<Member> MembersQuery() => _db.Members;
    public IQueryable<TrainerProfile> TrainersQuery() => _db.TrainerProfiles;
}