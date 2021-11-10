using System;

namespace Shuttle.Esb.Scheduling.Messages
{
	public class RegisterScheduleCommand
	{
	    public Guid Id { get; set; }
		public string Name { get; set; }
		public string CronExpression { get; set; }
		public string InboxWorkQueueUri { get; set; }
		public DateTime? NextNotification { get; set; }
	}
}