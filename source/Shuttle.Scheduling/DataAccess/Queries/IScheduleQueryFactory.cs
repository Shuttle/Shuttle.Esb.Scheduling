using System;
using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
	public interface IScheduleQueryFactory
	{
		IQuery All();
		IQuery HasScheduleStructures();
		IQuery Add(string name, string inboxWorkQueueUri, string cronExpression, DateTime nextNotification);
		IQuery Remove(string name);
		IQuery Contains(string name);
		IQuery Save(string name, string inboxWorkQueueUri, string cronExpression, DateTime nextNotification);
		IQuery SaveNextNotification(string name, DateTime nextNotification);
	}
}