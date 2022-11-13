using System;
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
            return _dataRepository.FetchItems(_queryFactory.All());
        }

        public void SetNextNotification(Guid id, DateTime nextNotification)
        {
            _databaseGateway.Execute(_queryFactory.SetNextNotification(id, nextNotification));
        }

        public void Save(Schedule schedule)
        {
            _databaseGateway.Execute(_queryFactory.Save(schedule));
        }

        public void Remove(Guid id)
        {
            _databaseGateway.Execute(_queryFactory.Remove(id));
        }

        public bool Contains(string name, string inboxWorkQueueUri, string cronExpression)
        {
            return _databaseGateway.GetScalar<int>(_queryFactory.Contains(name, inboxWorkQueueUri,
                       cronExpression)) == 1;
        }
    }
}