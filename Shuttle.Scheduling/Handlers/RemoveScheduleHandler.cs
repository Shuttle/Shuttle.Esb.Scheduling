using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling
{
	public class RemoveScheduleHandler : IMessageHandler<RemoveScheduleCommand>
	{
		private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public RemoveScheduleHandler(IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

			_databaseContextFactory = databaseContextFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(HandlerContext<RemoveScheduleCommand> context)
		{
			var command = context.Message;

			using (_databaseContextFactory.Create(SchedulingData.ConnectionStringName))
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