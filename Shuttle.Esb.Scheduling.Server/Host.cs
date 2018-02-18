using System;
using System.Threading;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
using Shuttle.Core.Data;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.ServiceHost;
using Shuttle.Core.Threading;
using Shuttle.Esb.Scheduling.Messages;

namespace Shuttle.Esb.Scheduling.Server
{
    public class Host : IServiceHost, IThreadState
    {
        private readonly WindsorContainer _container = new WindsorContainer();
        private IServiceBus _bus;
        private ISchedulingConfiguration _configuration;
        private IDatabaseContextFactory _databaseContextFactory;
        private IScheduleRepository _repository;

        private volatile bool _running = true;
        private Thread _thread;

        public Host()
        {
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Host))));
        }

        public void Start()
        {
            var container = new WindsorComponentContainer(_container);

            container.RegisterSuffixed("Shuttle.Esb.Scheduling");
            container.RegisterInstance(SchedulingSection.Configuration());

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();

            _repository = _container.Resolve<IScheduleRepository>();
            _configuration = _container.Resolve<ISchedulingConfiguration>();
            _databaseContextFactory = _container.Resolve<IDatabaseContextFactory>();

            _thread = new Thread(ProcessSchedule);

            _thread.Start();
        }

        public void Stop()
        {
            _running = false;

            _bus?.Dispose();

            if (_thread != null && _thread.IsAlive)
            {
                _thread.Join();
            }

            _container?.Dispose();

            LogManager.Shutdown();
        }

        public bool Active => _running;

        private void ProcessSchedule()
        {
            var ms = _configuration.SecondsBetweenScheduleChecks * 1000;

            while (_running)
            {
                using (_databaseContextFactory.Create(_configuration.ProviderName, _configuration.ConnectionString))
                {
                    foreach (var schedule in _repository.All())
                    {
                        if (!schedule.ShouldSendNotification)
                        {
                            continue;
                        }

                        _bus.Send(new RunScheduleCommand
                        {
                            Name = schedule.Name,
                            DateDue = schedule.NextNotification,
                            DateSent = DateTime.Now,
                            ServerName = Environment.MachineName
                        }, c => c.WithRecipient(schedule.InboxWorkQueueUri));

                        schedule.SetNextNotification();

                        _repository.SaveNextNotification(schedule);
                    }
                }

                ThreadSleep.While(ms, this);
            }
        }
    }
}