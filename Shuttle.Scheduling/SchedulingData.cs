using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
	public class SchedulingData
	{
		public static readonly DataSource Source = new DataSource("Scheduling", new SqlDbDataParameterFactory());
	}
}