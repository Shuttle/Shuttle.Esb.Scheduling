using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;

namespace Shuttle.Esb.Scheduling
{
	public class RemoveScheduleHandler : IMessageHandler<RemoveScheduleCommand>
	{
	    private readonly ISchedulingConfiguration _configuration;
	    private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public RemoveScheduleHandler(ISchedulingConfiguration configuration, IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(configuration, "configuration");
			Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

		    _configuration = configuration;
		    _databaseContextFactory = databaseContextFactory;
			_scheduleRepository = scheduleRepository;
		}

		public void ProcessMessage(IHandlerContext<RemoveScheduleCommand> context)
		{
			var command = context.Message;

			using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
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