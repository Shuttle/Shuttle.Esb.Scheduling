using System;
using Shuttle.Core.Data;

namespace Shuttle.Esb.Scheduling
{
	public interface IScheduleQueryFactory
	{
		IQuery All();
		IQuery HasScheduleStructures();
		IQuery Remove(Guid id);
		IQuery Contains(string name, string cronExpression);
		IQuery Save(Schedule schedule);
		IQuery SetNextNotification(Guid id, DateTime nextNotification);
	    IQuery Search(Query.Schedule.Specification specification);
	}
}