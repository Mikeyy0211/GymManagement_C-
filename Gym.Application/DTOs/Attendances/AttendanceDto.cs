using Gym.Core.Entities;

namespace Gym.Application.DTOs.Attendances;

public record AttendanceDto(Guid Id, AttendanceStatus Status, DateTime? CheckInTime);