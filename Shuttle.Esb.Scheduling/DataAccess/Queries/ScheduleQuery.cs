using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.Esb.Scheduling
{
	public class ScheduleQuery : IScheduleQuery
	{
		private readonly IDatabaseGateway _databaseGateway;
		private readonly IScheduleQueryFactory _queryFactory;

		public ScheduleQuery(IDatabaseGateway databaseGateway, IScheduleQueryFactory queryFactory)
		{
			Guard.AgainstNull(databaseGateway, nameof(databaseGateway));
			Guard.AgainstNull(queryFactory, nameof(queryFactory));

			_databaseGateway = databaseGateway;
			_queryFactory = queryFactory;
		}

		public bool HasScheduleStructures()
		{
			return _databaseGateway.GetScalarUsing<int>(_queryFactory.HasScheduleStructures()) == 1;
		}
	}
}