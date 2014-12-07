using System;
using Shuttle.Core.Data;
using Shuttle.Core.Domain;
using Shuttle.Core.Infrastructure;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling
{
	public class SendNotificationDomainHandler : IDomainEventHandler<SendNotification>
	{
		private readonly IScheduleRepository scheduleRepository;
		private readonly ILog _log;

		public SendNotificationDomainHandler(IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(scheduleRepository, "scheduleRepository");

			this.scheduleRepository = scheduleRepository;

			_log = Log.For(this);
		}

		public IServiceBus Bus { get; set; }
		public IDatabaseConnectionFactory DatabaseConnectionFactory { get; set; }

		public void Handle(SendNotification args)
		{
			using (DatabaseConnectionFactory.Create(SchedulerData.Source))
			{
				var message = new RunScheduleCommand
								{
									Name = args.Schedule.Name,
									DateDue = args.Due,
									DateSent = DateTime.Now,
									ServerName = Environment.MachineName
								};

				scheduleRepository.SaveNextNotification(args.Schedule);

				Bus.Send(message, c=> c.WithRecipient(args.Schedule.InboxWorkQueueUri));

				_log.Debug(string.Format("[RunScheduleCommand] : name '{0}' / inbox work queue uri = '{1}'", message.Name, args.Schedule.InboxWorkQueueUri));
			}
		}
	}
}