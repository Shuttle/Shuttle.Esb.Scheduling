using Shuttle.Core.Data;

namespace Shuttle.Scheduling
{
	public interface IScheduleQuery
	{
		bool HasScheduleStructures(DataSource source);
	}
}