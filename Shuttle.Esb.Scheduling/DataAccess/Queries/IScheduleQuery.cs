using System.Collections.Generic;

namespace Shuttle.Esb.Scheduling
{
    public interface IScheduleQuery
    {
        bool HasScheduleStructures();
        IEnumerable<Query.Schedule> Search(Query.Schedule.Specification specification);
    }
}