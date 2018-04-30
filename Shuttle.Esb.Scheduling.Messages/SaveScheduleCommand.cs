using System;

namespace Shuttle.Esb.Scheduling.Messages
{
	public class SaveScheduleCommand
	{
	    public Guid Id { get; set; }
		public string Name { get; set; }
		public string CronExpression { get; set; }
		public string InboxWorkQueueUri { get; set; }
	}
}