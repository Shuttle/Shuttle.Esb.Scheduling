using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Core.Reflection;
using Shuttle.Esb.Scheduling.Messages;

namespace Shuttle.Esb.Scheduling
{
    public class SchedulingHostedService : IHostedService
    {
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IOptionsMonitor<ConnectionStringOptions> _connectionStringOptions;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly ILogger<SchedulingHostedService> _logger;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly SchedulingOptions _schedulingOptions;
        private readonly IServiceBus _serviceBus;
        private Thread _thread;

        public SchedulingHostedService(IOptions<SchedulingOptions> schedulingOptions, IOptionsMonitor<ConnectionStringOptions> connectionStringOptions, ILogger<SchedulingHostedService> logger, IServiceBus serviceBus, IDatabaseContextFactory databaseContextFactory, IScheduleRepository scheduleRepository)
        {
            Guard.AgainstNull(schedulingOptions, nameof(schedulingOptions));
            Guard.AgainstNull(schedulingOptions.Value, nameof(schedulingOptions.Value));
            Guard.AgainstNull(connectionStringOptions, nameof(connectionStringOptions));
            Guard.AgainstNull(logger, nameof(logger));
            Guard.AgainstNull(serviceBus, nameof(serviceBus));

            _schedulingOptions = schedulingOptions.Value;
            _connectionStringOptions = connectionStringOptions;
            _logger = logger;
            _serviceBus = serviceBus;
            _databaseContextFactory = databaseContextFactory;
            _scheduleRepository = scheduleRepository;
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _thread = new Thread(ProcessSchedule);

            _thread.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            _thread.Join(TimeSpan.FromSeconds(30));

            return Task.CompletedTask;
        }

        private void ProcessSchedule()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                using (_databaseContextFactory.Create(_connectionStringOptions.Get(_schedulingOptions.ConnectionStringName).ProviderName, _connectionStringOptions.Get(_schedulingOptions.ConnectionStringName).ConnectionString))
                {
                    foreach (var schedule in _scheduleRepository.All())
                    {
                        if (!schedule.ShouldSendNotification)
                        {
                            continue;
                        }

                        try
                        {
                            _serviceBus.Publish(new ScheduleNotification
                            {
                                Name = schedule.Name,
                                NotificationDate = schedule.NextNotification,
                                ServerName = Environment.MachineName
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.AllMessages());
                        }

                        schedule.SetNextNotification();

                        _scheduleRepository.SetNextNotification(schedule);
                    }
                }

                try
                {
                    Task.Delay(_schedulingOptions.ScheduleProcessingInterval, _cancellationToken).Wait(_cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
            }
        }
    }
}