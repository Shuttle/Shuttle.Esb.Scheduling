using Shuttle.Core.Contract;

namespace Shuttle.Esb.Scheduling
{
    public static class ScheduleRepositoryExtensions
    {
        public static void SetNextNotification(this IScheduleRepository repository, Schedule schedule)
        {
            Guard.AgainstNull(repository, nameof(repository));
            Guard.AgainstNull(schedule, nameof(schedule));
            
            repository.SetNextNotification(schedule.Id, schedule.NextNotification);
        }
    }
}