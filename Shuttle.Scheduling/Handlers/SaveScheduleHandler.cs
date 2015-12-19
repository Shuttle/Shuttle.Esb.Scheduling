using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling
{
	public class SaveScheduleHandler : IMessageHandler<SaveScheduleCommand>
	{
		private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public SaveScheduleHandler(IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

			_databaseContextFactory = databaseContextFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(IHandlerContext<SaveScheduleCommand> context)
		{
			var command = context.Message;

			using (_databaseContextFactory.Create(SchedulingData.ConnectionStringName))
			{
				_scheduleRepository.Save(new Schedule(command.Name, command.InboxWorkQueueUri, command.CronExpression));
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}