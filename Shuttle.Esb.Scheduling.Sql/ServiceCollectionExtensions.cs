using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;
using Shuttle.Esb.Scheduling.Sql.SqlServer;

namespace Shuttle.Esb.Scheduling.Sql
{
    public static class SchedulingBuilderExtensions
    {
        public static SchedulingBuilder AddSqlServer(this SchedulingBuilder builder)
        {
            Guard.AgainstNull(builder, nameof(builder));

            builder.Services.AddSingleton<IScheduleQueryFactory, ScheduleQueryFactory>();

            return builder;
        }
    }
}