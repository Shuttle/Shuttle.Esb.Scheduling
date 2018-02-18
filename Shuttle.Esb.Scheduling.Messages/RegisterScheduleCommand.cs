namespace Shuttle.Esb.Scheduling.Messages
{
	public class RegisterScheduleCommand
	{
		public string Name { get; set; }
		public string CronExpression { get; set; }
		public string InboxWorkQueueUri { get; set; }
	}
}