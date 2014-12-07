using System.Collections.Generic;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling
{
	public class ScheduleRepository : IScheduleRepository
	{
		private readonly IDatabaseGateway _databaseGateway;
		private readonly IDataRepository<Schedule> _dataRepository;
		private readonly IScheduleQueryFactory _queryFactory;

		public ScheduleRepository(IDatabaseGateway databaseGateway, IDataRepository<Schedule> dataRepository, IScheduleQueryFactory queryFactory)
		{
			Guard.AgainstNull(databaseGateway, "databaseGateway");
			Guard.AgainstNull(dataRepository, "dataRepository");
			Guard.AgainstNull(queryFactory, "queryFactory");

			_databaseGateway = databaseGateway;
			_dataRepository = dataRepository;
			_queryFactory = queryFactory;
		}

		public IEnumerable<Schedule> All()
		{
			return _dataRepository.FetchAllUsing(SchedulerData.Source, _queryFactory.All());
		}

		public void SaveNextNotification(Schedule schedule)
		{
			_databaseGateway.ExecuteUsing(SchedulerData.Source, _queryFactory.SaveNextNotification(schedule.Name, schedule.NextNotification));
		}
	}
}