using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling
{
	public class RemoveScheduleHandler : IMessageHandler<RemoveScheduleCommand>
	{
		private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public RemoveScheduleHandler(IDatabaseConnectionFactory databaseConnectionFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(databaseConnectionFactory, "databaseConnectionFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

			_databaseConnectionFactory = databaseConnectionFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(HandlerContext<RemoveScheduleCommand> context)
		{
			var command = context.Message;

			using (_databaseConnectionFactory.Create(SchedulingData.Source))
			{
				_scheduleRepository.Remove(command.Name);
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}