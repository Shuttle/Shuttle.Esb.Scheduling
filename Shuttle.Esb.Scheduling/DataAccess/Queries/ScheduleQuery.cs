using System.Collections.Generic;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Esb.Scheduling
{
	public class ScheduleQuery : IScheduleQuery
	{
		private readonly IDatabaseGateway _databaseGateway;
	    private readonly IQueryMapper _queryMapper;
	    private readonly IScheduleQueryFactory _queryFactory;

		public ScheduleQuery(IDatabaseGateway databaseGateway, IQueryMapper queryMapper, IScheduleQueryFactory queryFactory)
		{
			Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
			Guard.AgainstNull(queryMapper, nameof(queryMapper));
			Guard.AgainstNull(queryFactory, nameof(queryFactory));

			_databaseGateway = databaseGateway;
		    _queryMapper = queryMapper;
		    _queryFactory = queryFactory;
		}

		public bool HasScheduleStructures()
		{
			return _databaseGateway.GetScalar<int>(_queryFactory.HasScheduleStructures()) == 1;
		}

		public IEnumerable<Query.Schedule> Search(Query.Schedule.Specification specification)
		{
			return _queryMapper.MapObjects<Query.Schedule>(_queryFactory.Search(specification));
		}
	}
}