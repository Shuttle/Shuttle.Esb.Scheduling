using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb;
using Shuttle.Esb.Scheduling.Messages;

namespace Shuttle.Esb.Scheduling
{
	public class RemoveScheduleHandler : IMessageHandler<RemoveScheduleCommand>
	{
	    private readonly ISchedulingConfiguration _configuration;
	    private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public RemoveScheduleHandler(ISchedulingConfiguration configuration, IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(configuration, nameof(configuration));
			Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
			Guard.AgainstNull(scheduleRepository, nameof(scheduleRepository));

		    _configuration = configuration;
		    _databaseContextFactory = databaseContextFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(IHandlerContext<RemoveScheduleCommand> context)
		{
			var message = context.Message;

			using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
			{
				_scheduleRepository.Remove(message.Id);
			}
		}
	}
}