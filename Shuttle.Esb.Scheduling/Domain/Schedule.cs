using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Cron;

namespace Shuttle.Esb.Scheduling
{
    public class Schedule
    {
        private readonly CronExpression _cronExpression;

        public Schedule(Guid id, string name, string inboxWorkQueueUri, string cronExpression,
            DateTime? nextNotification)
        {
            Guard.AgainstNullOrEmptyString(name, nameof(name));
            Guard.AgainstNullOrEmptyString(inboxWorkQueueUri, nameof(inboxWorkQueueUri));
            Guard.AgainstNullOrEmptyString(cronExpression, nameof(cronExpression));

            Id = id;
            Name = name;
            InboxWorkQueueUri = inboxWorkQueueUri;
            CronExpression = cronExpression;
            _cronExpression = new CronExpression(cronExpression);
            NextNotification = nextNotification ?? _cronExpression.NextOccurrence();
        }

        public Guid Id { get; }

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