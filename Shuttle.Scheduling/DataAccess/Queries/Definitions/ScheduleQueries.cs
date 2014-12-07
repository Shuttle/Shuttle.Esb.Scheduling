using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
	public class ScheduleQueries
	{
		public static IQuery HasScheduleStructures()
		{
			return
				RawQuery.Create("IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Schedule') select 1 ELSE select 0");
		}		
	}
}