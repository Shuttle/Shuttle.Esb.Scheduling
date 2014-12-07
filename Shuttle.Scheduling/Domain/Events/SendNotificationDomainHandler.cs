using System;
using Shuttle.Core.Data;
using Shuttle.Core.Domain;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling
{
	public class SendNotificationDomainHandler : IDomainEventHandler<SendNotification>
	{
		private readonly IServiceBus _bus;
		private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
		private readonly IScheduleRepository _scheduleRepository;

		private readonly ILog _log;

		public SendNotificationDomainHandler(IServiceBus bus, IDatabaseConnectionFactory databaseConnectionFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(bus, "bus");
			Guard.AgainstNull(databaseConnectionFactory, "databaseConnectionFactory");
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

			_scheduleRepository = scheduleRepository;
			_bus = bus;
			_databaseConnectionFactory = databaseConnectionFactory;

			_log = Log.For(this);
		}


		public void Handle(SendNotification args)
		{
			using (_databaseConnectionFactory.Create(SchedulerData.Source))
			{
				var message = new RunScheduleCommand
								{
									Name = args.Schedule.Name,
									DateDue = args.Due,
									DateSent = DateTime.Now,
									ServerName = Environment.MachineName
								};

				_scheduleRepository.SaveNextNotification(args.Schedule);

				_bus.Send(message, c => c.WithRecipient(args.Schedule.InboxWorkQueueUri));

				_log.For(this).Debug(string.Format("[RunScheduleCommand] : name = '{0}' / recipient inbox work queue uri = '{1}'.", message.Name, args.Schedule.InboxWorkQueueUri));
			}
		}
	}
}