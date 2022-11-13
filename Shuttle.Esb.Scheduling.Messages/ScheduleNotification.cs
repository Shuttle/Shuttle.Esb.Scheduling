using System;

namespace Shuttle.Esb.Scheduling.Messages
{
	public class ScheduleNotification
	{
		public string Name { get; set; }
		public string ServerName { get; set; }
		public DateTime NotificationDate { get; set; }
	}
}