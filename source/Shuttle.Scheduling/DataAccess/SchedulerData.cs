using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
    public class SchedulerData
    {
	    public static readonly DataSource Source = new DataSource("Scheduler", new SqlDbDataParameterFactory());
	}
}