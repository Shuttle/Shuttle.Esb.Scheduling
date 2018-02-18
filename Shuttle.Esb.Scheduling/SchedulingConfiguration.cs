namespace Shuttle.Esb.Scheduling
{
    public class SchedulingConfiguration : ISchedulingConfiguration
    {
        public SchedulingConfiguration()
        {
            SecondsBetweenScheduleChecks = 15;
        }

        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }
        public int SecondsBetweenScheduleChecks { get; set; }
    }
}