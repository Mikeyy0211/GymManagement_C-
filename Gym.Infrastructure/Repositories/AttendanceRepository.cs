using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Gym.Infrastructure.Persistence;

namespace Gym.Infrastructure.Repositories;
public class AttendanceRepository : BaseRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(GymDbContext ctx) : base(ctx) { }
}