using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Esb.Scheduling.Messages;

namespace Shuttle.Esb.Scheduling
{
	public class SaveScheduleHandler : IMessageHandler<SaveScheduleCommand>
	{
	    private readonly ISchedulingConfiguration _configuration;
	    private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public SaveScheduleHandler(ISchedulingConfiguration configuration, IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(configuration, nameof(configuration));
			Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
			Guard.AgainstNull(scheduleRepository, nameof(scheduleRepository));

		    _configuration = configuration;
		    _databaseContextFactory = databaseContextFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(IHandlerContext<SaveScheduleCommand> context)
		{
			var command = context.Message;

            using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
            {
                _scheduleRepository.Save(new Schedule(command.Name, command.InboxWorkQueueUri, command.CronExpression));
			}
		}
	}
}