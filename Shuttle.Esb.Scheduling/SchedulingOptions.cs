using System;

namespace Shuttle.Esb.Scheduling
{
    public class SchedulingOptions
    {
        public const string SectionName = "Shuttle:Scheduling";

        public string ConnectionStringName { get; set; }
        public TimeSpan ScheduleProcessingInterval { get; set; } = TimeSpan.FromSeconds(15);
        public bool SuppressHostedService { get; set; }
    }
}