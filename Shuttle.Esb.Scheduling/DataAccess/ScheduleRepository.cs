using System.Collections.Generic;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Scheduling
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
			return _dataRepository.FetchAllUsing(_queryFactory.All());
		}

		public void SaveNextNotification(Schedule schedule)
		{
			_databaseGateway.ExecuteUsing(_queryFactory.SaveNextNotification(schedule));
		}

		public void Save(Schedule schedule)
		{
			_databaseGateway.ExecuteUsing(_queryFactory.Save(schedule));
		}

		public void Remove(string name)
		{
			_databaseGateway.ExecuteUsing(_queryFactory.Remove(name));
		}
	}
}