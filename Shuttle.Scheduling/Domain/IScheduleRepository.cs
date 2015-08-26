using System.Collections.Generic;

namespace Shuttle.Scheduling
{
	public interface IScheduleRepository
	{
		IEnumerable<Schedule> All();
		void SaveNextNotification(Schedule schedule);
		void Save(Schedule schedule);
		void Remove(string name);
	}
}