using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling
{
	public class SaveScheduleHandler : IMessageHandler<SaveScheduleCommand>
	{
		private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public SaveScheduleHandler(IDatabaseConnectionFactory databaseConnectionFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(databaseConnectionFactory, "databaseConnectionFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

			_databaseConnectionFactory = databaseConnectionFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(HandlerContext<SaveScheduleCommand> context)
		{
			var command = context.Message;

			using (_databaseConnectionFactory.Create(SchedulingData.Source))
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