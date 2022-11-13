using System;
using Shuttle.Core.Data;
using Shuttle.Esb.Scheduling.DataAccess;

namespace Shuttle.Esb.Scheduling
{
    public class ScheduleQueryFactory : IScheduleQueryFactory
    {
        public IQuery All()
        {
            return RawQuery.Create(@"
select
    [Id],
	[Name],
	[InboxWorkQueueUri],
	[CronExpression],
	[NextNotification]
from
	[dbo].[Schedule]
order by
	[Name],
    [InboxWorkQueueUri]
");
        }

        public IQuery HasScheduleStructures()
        {
            return
                RawQuery.Create(
                    "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Schedule') select 1 ELSE select 0");
        }

        public IQuery Remove(Guid id)
        {
            return RawQuery.Create(@"delete from [dbo].[Schedule] where [Id] = @Id")
                .AddParameterValue(Columns.Id, id);
        }

        public IQuery Contains(string name, string inboxWorkQueueUri, string cronExpression)
        {
            return RawQuery
                .Create(@"
if exists 
(
    select 
        null 
    from 
        [dbo].[Schedule] 
    where 
        [Name] = @Name
    and
        [CronExpression] = @CronExpression
) 
    select 1 
else 
    select 0
")
                .AddParameterValue(Columns.Name, name)
                .AddParameterValue(Columns.CronExpression, cronExpression);
        }

        public IQuery Save(Schedule schedule)
        {
            return RawQuery.Create(@"
if not exists 
(
    select 
        null 
    from 
        [dbo].[Schedule] 
    where
        [Id] = @Id
)
	insert into [dbo].[Schedule]
	(
        [Id],
		[Name],
		[CronExpression],
		[NextNotification]
	)
	values
	(
        @Id,
		@Name,
		@CronExpression,
		@NextNotification
	)
else
	update 
        [dbo].[Schedule] 
    set
        [Name] = @Name,
		[CronExpression] = @CronExpression,
		[NextNotification] = @NextNotification
	where
		[Id] = @Id
")
                .AddParameterValue(Columns.Id, schedule.Id)
                .AddParameterValue(Columns.Name, schedule.Name)
                .AddParameterValue(Columns.CronExpression, schedule.CronExpression)
                .AddParameterValue(Columns.NextNotification, schedule.NextNotification);
        }

        public IQuery SetNextNotification(Guid id, DateTime nextNotification)
        {
            return RawQuery.Create(@"
update 
    [dbo].[Schedule] 
set
	[NextNotification] = @NextNotification
where
	[Id] = @Id
")
                .AddParameterValue(Columns.Id, id)
                .AddParameterValue(Columns.NextNotification, nextNotification);
        }

        public IQuery Search(Query.Schedule.Specification specification)
        {
            return RawQuery.Create(@"
select
    [Id],
	[Name],
	[CronExpression],
	[NextNotification]
from
	[dbo].[Schedule]
where
(
    @Match is null
or
    (
        [Name] like @Match
    or
        [CronExpression] like @Match
    )
)
and
(
    @Id is null
    or
    Id = @Id
)
order by
	[Name]
")
                .AddParameterValue(Columns.Id, specification.Id)
                .AddParameterValue(Columns.Match, string.IsNullOrWhiteSpace(specification.FuzzyMatch) ? null : $"%{specification.FuzzyMatch}%");
        }
    }
}