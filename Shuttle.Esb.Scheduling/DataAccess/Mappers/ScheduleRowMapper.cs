using System.Data;
using Shuttle.Core.Data;
using Shuttle.Esb.Scheduling.DataAccess;

namespace Shuttle.Esb.Scheduling
{
    public class ScheduleRowMapper : IDataRowMapper<Schedule>
	{
		public MappedRow<Schedule> Map(DataRow row)
		{
			return new MappedRow<Schedule>(row,
				new Schedule(
				    Columns.Id.MapFrom(row), 
				    Columns.Name.MapFrom(row),
					Columns.CronExpression.MapFrom(row),
					Columns.NextNotification.MapFrom(row)));
		}
	}
}