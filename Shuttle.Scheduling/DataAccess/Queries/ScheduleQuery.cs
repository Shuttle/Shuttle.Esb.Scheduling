using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
	public class ScheduleQuery : IScheduleQuery
	{
		public IDatabaseGateway DatabaseGateway { get; set; }
		public IDatabaseConnectionFactory DatabaseConnectionFactory { get; set; }

		public bool HasScheduleStructures(DataSource source)
		{
			using (DatabaseConnectionFactory.Create(source))
			{
				return DatabaseGateway.GetScalarUsing<int>(source, ScheduleQueries.HasScheduleStructures()) == 1;
			}
		}
	}
}