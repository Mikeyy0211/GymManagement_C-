namespace Gym.Application.Trainers;

using AutoMapper;
using Gym.Application.DTOs.Trainers;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

public class TrainerService
{
    private readonly ITrainerRepository _repo;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public TrainerService(
        ITrainerRepository repo,
        IUserRepository users,
        IMapper mapper)
    {
        _repo = repo;
        _users = users;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TrainerDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _repo.Query()
            .Include(t => t.User)
            .ToListAsync(ct);

        return _mapper.Map<IEnumerable<TrainerDto>>(list);
    }

    public async Task<TrainerDto> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var trainer = await _repo.GetByIdAsync(id, true, false, ct)
                      ?? throw new KeyNotFoundException("Trainer not found");

        return _mapper.Map<TrainerDto>(trainer);
    }

    public async Task<TrainerDto> CreateAsync(CreateTrainerRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId)
                   ?? throw new KeyNotFoundException("User not found");

        var trainer = _mapper.Map<TrainerProfile>(req);

        await _repo.AddAsync(trainer, ct);

        return _mapper.Map<TrainerDto>(trainer);
    }

    public async Task UpdateAsync(Guid id, UpdateTrainerRequest req, CancellationToken ct)
    {
        var trainer = await _repo.GetByIdAsync(id, false, false, ct)
                      ?? throw new KeyNotFoundException("Trainer not found");

        _mapper.Map(req, trainer);

        await _repo.UpdateAsync(trainer, ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct)
    {
        var trainer = await _repo.GetByIdAsync(id, false, false, ct)
                      ?? throw new KeyNotFoundException("Trainer not found");

        await _repo.SoftDeleteAsync(trainer, ct);
    }
}
