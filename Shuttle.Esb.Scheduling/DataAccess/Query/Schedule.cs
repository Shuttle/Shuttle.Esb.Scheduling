using System;

namespace Shuttle.Esb.Scheduling.Query
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string InboxWorkQueueUri { get; set; }
        public string CronExpression { get; set; }
        public DateTime NextNotification { get; set; }

        public class Specification
        {
            public Guid? Id { get; private set; }

            public string FuzzyMatch { get; private set; }

            public Specification MatchingFuzzy(string fuzzyMatch)
            {
                FuzzyMatch = fuzzyMatch;

                return this;
            }

            public Specification WithId(Guid id)
            {
                Id = id;

                return this;
            }
        }
    }
}