using System;
using System.Collections.Generic;

namespace Shuttle.Esb.Scheduling
{
	public interface IScheduleRepository
	{
		IEnumerable<Schedule> All();
		void SetNextNotification(Guid id, DateTime nextNotification);
		void Save(Schedule schedule);
		void Remove(Guid id);
	    bool Contains(string name, string cronExpression);
	}
}