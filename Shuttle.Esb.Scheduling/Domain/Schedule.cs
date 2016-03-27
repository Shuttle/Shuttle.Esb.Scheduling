using System;

namespace Shuttle.Esb.Scheduling
{
    public class Schedule 
    {
	    internal Schedule(string name, string inboxWOrkQueueUri, string cronExpression)
		    : this(name, inboxWOrkQueueUri, cronExpression, null)
	    {
	    }

	    internal Schedule(string name, string inboxWOrkQueueUri, string cronExpression, DateTime? nextNotification)
        {
	        Name = name;
	        InboxWorkQueueUri = inboxWOrkQueueUri;
            CronExpression = cronExpression;
            _cronExpression = new CronExpression(cronExpression);
			NextNotification = nextNotification ?? _cronExpression.NextOccurrence();
        }

	    public string Name { get; private set; }
	    public string InboxWorkQueueUri { get; private set; }
        public string CronExpression { get; private set; }
        public DateTime NextNotification { get; private set; }

	    private readonly CronExpression _cronExpression;

        protected virtual bool ShouldSendNotification
        {
            get { return DateTime.Now >= NextNotification; }
        }

        public void SetNextNotification()
        {
			NextNotification = _cronExpression.NextOccurrence();
        }

        public RunScheduleCommand Notification()
        {
            if (!ShouldSendNotification)
            {
                return null;
            }

            var due = NextNotification;

            SetNextNotification();

			return new RunScheduleCommand
			{
				Name = Name,
				DateDue = due,
				DateSent = DateTime.Now,
				ServerName = Environment.MachineName
			};
		}
    }
}