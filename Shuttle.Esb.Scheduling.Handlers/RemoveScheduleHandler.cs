﻿using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Esb.Scheduling.Messages;

namespace Shuttle.Esb.Scheduling.Handlers
{
	public class RemoveScheduleHandler : IMessageHandler<RemoveSchedule>
	{
		private readonly IDatabaseContextFactory _databaseContextFactory;
		private readonly IScheduleRepository _scheduleRepository;
		private readonly SchedulingOptions _schedulingOptions;
		private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;

		public RemoveScheduleHandler(IOptions<SchedulingOptions> schedulingOptions, IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
		{
			Guard.AgainstNull(schedulingOptions, nameof(schedulingOptions));
			Guard.AgainstNull(schedulingOptions.Value, nameof(schedulingOptions.Value));
			Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
			Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
			Guard.AgainstNull(scheduleRepository, nameof(scheduleRepository));

			_schedulingOptions = schedulingOptions.Value;
			_connectionStringOptions = connectionStringOptions;
			_databaseContextFactory = databaseContextFactory;
			_scheduleRepository = scheduleRepository;
		}

        public void ProcessMessage(IHandlerContext<RemoveSchedule> context)
		{
			var message = context.Message;

			using (_databaseContextFactory.Create(_connectionStringOptions.Get(_schedulingOptions.ConnectionStringName).ProviderName, _connectionStringOptions.Get(_schedulingOptions.ConnectionStringName).ConnectionString))
            {
				_scheduleRepository.Remove(message.Id);
			}
		}
	}
}