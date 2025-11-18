using AutoMapper;
using Gym.Application.DTOs.Classes;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Classes;

public class ClassService
{
    private readonly IClassRepository _classes;
    private readonly ISessionRepository _sessions;
    private readonly IMapper _mapper;

    public ClassService(
        IClassRepository classes,
        ISessionRepository sessions,
        IMapper mapper)
    {
        _classes = classes;
        _sessions = sessions;
        _mapper = mapper;
    }


    public async Task<ClassDto> CreateClassAsync(CreateClassRequest rq, CancellationToken ct)
    {
        var cls = _mapper.Map<GymClass>(rq);
        await _classes.AddAsync(cls, ct);
        return _mapper.Map<ClassDto>(cls);
    }

   
    public async Task<IEnumerable<ClassDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _classes.Query()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<ClassDto>>(list);
    }

        public async Task<SessionDto> CreateSessionAsync(CreateSessionRequest rq, CancellationToken ct)
    {
        var cls = await _classes.GetByIdAsync(
            rq.ClassId,
            asNoTracking: false,
            includeDeleted: false,
            ct
        );

        if (cls == null)
            throw new KeyNotFoundException("Class not found");

        var endTime = rq.EndAt ?? rq.StartAt.AddHours(1);

        var session = new ClassSession
        {
            GymClassId = cls.Id,
            StartAt = rq.StartAt,
            EndAt = endTime,
            CapacityOverride = rq.CapacityOverride
        };

        await _sessions.AddAsync(session, ct);

        return _mapper.Map<SessionDto>(session);
    }

    public async Task<IEnumerable<SessionDto>> GetSessionsAsync(Guid classId, CancellationToken ct)
    {
        var list = await _sessions.Query()
            .Where(x => !x.IsDeleted && x.GymClassId == classId)
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<SessionDto>>(list);
    }
}
