using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.Esb.Scheduling
{
	public class ScheduleRowMapper : IDataRowMapper<Schedule>
	{
		public MappedRow<Schedule> Map(DataRow row)
		{
			return new MappedRow<Schedule>(row,
				new Schedule(
				    ScheduleColumns.Id.MapFrom(row), 
				    ScheduleColumns.Name.MapFrom(row),
					ScheduleColumns.InboxWorkQueueUri.MapFrom(row),
					ScheduleColumns.CronExpression.MapFrom(row),
					ScheduleColumns.NextNotification.MapFrom(row)));
		}
	}
}