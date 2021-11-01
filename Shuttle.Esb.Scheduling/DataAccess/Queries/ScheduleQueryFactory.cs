using System;
using Shuttle.Core.Data;

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
                .AddParameterValue(ScheduleColumns.Id, id);
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
        [InboxWorkQueueUri] = @InboxWorkQueueUri
    and
        [CronExpression] = @CronExpression
) 
    select 1 
else 
    select 0
")
                .AddParameterValue(ScheduleColumns.Name, name)
                .AddParameterValue(ScheduleColumns.InboxWorkQueueUri, inboxWorkQueueUri)
                .AddParameterValue(ScheduleColumns.CronExpression, cronExpression);
        }

        public IQuery Save(Schedule schedule)
        {
            return RawQuery.Create(@"
if not exists (select null from [dbo].[Schedule] where [Name] = @Name)
	insert into [dbo].[Schedule]
	(
        [Id],
		[Name],
		[InboxWorkQueueUri],
		[CronExpression],
		[NextNotification]
	)
	values
	(
        @Id,
		@Name,
		@InboxWorkQueueUri,
		@CronExpression,
		@NextNotification
	)
else
	update 
        [dbo].[Schedule] 
    set
        [Name] = @Name,
		[InboxWorkQueueUri] = @InboxWorkQueueUri,
		[CronExpression] = @CronExpression,
		[NextNotification] = @NextNotification
	where
		[Id] = @Id
")
                .AddParameterValue(ScheduleColumns.Id, schedule.Id)
                .AddParameterValue(ScheduleColumns.Name, schedule.Name)
                .AddParameterValue(ScheduleColumns.InboxWorkQueueUri, schedule.InboxWorkQueueUri)
                .AddParameterValue(ScheduleColumns.CronExpression, schedule.CronExpression)
                .AddParameterValue(ScheduleColumns.NextNotification, schedule.NextNotification);
        }

        public IQuery SaveNextNotification(Schedule schedule)
        {
            return RawQuery.Create(@"
update 
    [dbo].[Schedule] 
set
	[NextNotification] = @NextNotification
where
	[Id] = @Id
")
                .AddParameterValue(ScheduleColumns.Id, schedule.Id)
                .AddParameterValue(ScheduleColumns.NextNotification, schedule.NextNotification);
        }

        public IQuery Search(Query.Schedule.Specification specification)
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
where
    [Name] like @Match
or
    [InboxWorkQueueUri] like @Match
or
    [CronExpression] like @Match
order by
	[Name],
    [InboxWorkQueueUri]
")
                .AddParameterValue(ScheduleColumns.Match, string.IsNullOrWhiteSpace(specification.FuzzyMatch) ? null : $"%{specification.FuzzyMatch}%");
        }
    }
}