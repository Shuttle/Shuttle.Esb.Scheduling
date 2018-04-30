using System;
using System.Collections.Generic;

namespace Shuttle.Esb.Scheduling
{
	public interface IScheduleRepository
	{
		IEnumerable<Schedule> All();
		void SaveNextNotification(Schedule schedule);
		void Save(Schedule schedule);
		void Remove(Guid id);
	    bool Contains(string name, string inboxWorkQueueUri, string cronExpression);
	}
}