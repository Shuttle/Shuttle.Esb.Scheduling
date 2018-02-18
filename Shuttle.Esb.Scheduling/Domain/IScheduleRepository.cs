using System.Collections.Generic;

namespace Shuttle.Esb.Scheduling
{
	public interface IScheduleRepository
	{
		IEnumerable<Schedule> All();
		void SaveNextNotification(Schedule schedule);
		void Register(Schedule schedule);
		void Remove(string name);
	}
}