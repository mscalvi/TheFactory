using FurmaIdle.Helpers;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface ISpecialitiesService
    {
        (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec);
    }

    public sealed class SpecialitiesService : ISpecialitiesService
    {

        // Timers de Specialties: specialtyId -> (EndsAt, TotalSec)
        private readonly Dictionary<string, (DateTimeOffset endsAt, double totalSec)> _specTimers
            = new(StringComparer.Ordinal);

        // Specialties
        public (double Actual, double Total) GetSpecialtyTimer(string specialtyId)
        {
            if (string.IsNullOrWhiteSpace(specialtyId)) return (0, 0);

            if (_specTimers.TryGetValue(specialtyId, out var t))
            {
                var now = DateTimeOffset.UtcNow;
                var remaining = (t.endsAt - now).TotalSeconds;
                if (remaining <= 0)
                {
                    _specTimers.Remove(specialtyId);
                    return (0, t.totalSec);
                }
                return (remaining, t.totalSec);
            }
            return (0, 0);
        }
        public (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec)
            => spec is null ? (0, 0) : GetSpecialtyTimer(spec.Id);
    }
}
