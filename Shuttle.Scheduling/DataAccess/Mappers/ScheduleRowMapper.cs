using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
	public class ScheduleRowMapper : IDataRowMapper<Schedule>
	{
		public MappedRow<Schedule> Map(DataRow row)
		{
			return new MappedRow<Schedule>(row,
				new Schedule(ScheduleColumns.Name.MapFrom(row),
					ScheduleColumns.InboxWorkQueueUri.MapFrom(row),
					ScheduleColumns.CronExpression.MapFrom(row),
					ScheduleColumns.NextNotification.MapFrom(row)));
		}
	}
}