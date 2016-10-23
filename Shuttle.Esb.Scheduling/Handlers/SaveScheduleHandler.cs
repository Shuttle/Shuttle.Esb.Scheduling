using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Esb;

namespace Shuttle.Esb.Scheduling
{
	public class SaveScheduleHandler : IMessageHandler<SaveScheduleCommand>
	{
	    private readonly ISchedulingConfiguration _configuration;
	    private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;

		public SaveScheduleHandler(ISchedulingConfiguration configuration, IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(configuration, "configuration");
			Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

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

		public bool IsReusable
		{
			get { return true; }
		}
	}
}