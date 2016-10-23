namespace Shuttle.Esb.Scheduling
{
    public class SchedulingConfiguration : ISchedulingConfiguration
    {
        public SchedulingConfiguration()
        {
            MillisecondsBetweenScheduleChecks = 5000;
        }

        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
        public int MillisecondsBetweenScheduleChecks { get; set; }
    }
}