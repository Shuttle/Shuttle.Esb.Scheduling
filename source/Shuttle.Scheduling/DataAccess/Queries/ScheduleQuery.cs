using System.Data;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling
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

		public DataTable All(DataSource source)
		{
			return _databaseGateway.GetDataTableFor(source, _queryFactory.All());
		}

		public bool HasScheduleStructures(DataSource source)
		{
			return _databaseGateway.GetScalarUsing<int>(source, _queryFactory.HasScheduleStructures()) == 1;
		}
	}
}