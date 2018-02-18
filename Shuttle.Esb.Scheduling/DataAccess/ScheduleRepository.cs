using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Esb.Scheduling
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IDataRepository<Schedule> _dataRepository;
        private readonly IScheduleQueryFactory _queryFactory;

        public ScheduleRepository(IDatabaseGateway databaseGateway, IDataRepository<Schedule> dataRepository,
            IScheduleQueryFactory queryFactory)
        {
            Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
            Guard.AgainstNull(dataRepository, nameof(dataRepository));
            Guard.AgainstNull(queryFactory, nameof(queryFactory));

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

        public void Register(Schedule schedule)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Register(schedule));
        }

        public void Remove(string name)
        {
            _databaseGateway.ExecuteUsing(_queryFactory.Remove(name));
        }
    }
}