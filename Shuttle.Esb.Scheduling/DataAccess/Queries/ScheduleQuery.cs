using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Scheduling
{
	public class ScheduleQuery : IScheduleQuery
	{
		private readonly IDatabaseGateway _databaseGateway;
		private readonly IScheduleQueryFactory _queryFactory;

		public ScheduleQuery(IDatabaseGateway databaseGateway, IScheduleQueryFactory queryFactory)
		{
			Guard.AgainstNull(databaseGateway, "databaseGateway");
			Guard.AgainstNull(queryFactory, "queryFactory");

			_databaseGateway = databaseGateway;
			_queryFactory = queryFactory;
		}

		public bool HasScheduleStructures()
		{
			return _databaseGateway.GetScalarUsing<int>(_queryFactory.HasScheduleStructures()) == 1;
		}
	}
}