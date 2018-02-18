using System;
using Shuttle.Core.Cron;

namespace Shuttle.Esb.Scheduling
{
    public class Schedule
    {
        private readonly CronExpression _cronExpression;

        public Schedule(string name, string inboxWOrkQueueUri, string cronExpression)
            : this(name, inboxWOrkQueueUri, cronExpression, null)
        {
        }

        public Schedule(string name, string inboxWOrkQueueUri, string cronExpression, DateTime? nextNotification)
        {
            Name = name;
            InboxWorkQueueUri = inboxWOrkQueueUri;
            CronExpression = cronExpression;
            _cronExpression = new CronExpression(cronExpression);
            NextNotification = nextNotification ?? _cronExpression.NextOccurrence();
        }

        public string Name { get; }
        public string InboxWorkQueueUri { get; }
        public string CronExpression { get; }
        public DateTime NextNotification { get; private set; }

        public bool ShouldSendNotification => DateTime.Now >= NextNotification;

        public void SetNextNotification()
        {
            NextNotification = _cronExpression.NextOccurrence();
        }
    }
}