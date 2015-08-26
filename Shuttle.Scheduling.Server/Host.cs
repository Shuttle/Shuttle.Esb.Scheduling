using System;
using System.Threading;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;

namespace Shuttle.Scheduling.Server
{
	public class Host : IHost, IDisposable, IThreadState
	{
		private readonly WindsorContainer _container = new WindsorContainer();

		private IServiceBus _bus;

		private volatile bool _running = true;
		private Thread _thread;
		private IDatabaseConnectionFactory _databaseConnectionFactory;
		private IScheduleRepository _repository;

		private readonly int millisecondsBetweenScheduleChecks =
			ConfigurationItem<int>.ReadSetting("MillisecondsBetweenScheduleChecks", 5000).GetValue();

		public void Dispose()
		{
			_running = false;

			if (_thread != null && _thread.IsAlive)
			{
				_thread.Join();
			}

			if (_bus != null)
			{
				_bus.Dispose();
			}

			if (_container != null)
			{
				_container.Dispose();
			}

			LogManager.Shutdown();
		}

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (Program))));

			new ConnectionStringService().Approve();

			_container.Register(Component.For<IDatabaseConnectionCache>()
				.ImplementedBy<ThreadStaticDatabaseConnectionCache>());

			_container.Register(Component.For<IDbConnectionConfiguration>()
				.ImplementedBy<DbConnectionConfiguration>());

			_container.Register(Component.For<IDbConnectionConfigurationProvider>()
				.ImplementedBy<DbConnectionConfigurationProvider>());

			_container.Register(Component.For<IDatabaseGateway>().ImplementedBy<DatabaseGateway>());
			_container.Register(Component.For<IDatabaseConnectionFactory>().ImplementedBy<DatabaseConnectionFactory>());
			_container.Register(Component.For(typeof (IDataRepository<>)).ImplementedBy(typeof (DataRepository<>)));

			_container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Core.Data")
					.Pick()
					.If(type => type.Name.EndsWith("Factory"))
					.Configure(configurer => configurer.Named(configurer.Implementation.Name.ToLower()))
					.WithService.Select((type, basetype) => new[] {type.InterfaceMatching(@".*Factory\Z")}));

			const string shuttleScheduling = "Shuttle.Scheduling";

			_container.Register(
				Classes
					.FromAssemblyNamed(shuttleScheduling)
					.BasedOn(typeof (IDataRowMapper<>))
					.WithServiceFirstInterface());

			_container.Register(
				Classes
					.FromAssemblyNamed(shuttleScheduling)
					.Pick()
					.If(type => type.Name.EndsWith("Repository"))
					.WithServiceFirstInterface());

			_container.Register(
				Classes
					.FromAssemblyNamed(shuttleScheduling)
					.Pick()
					.If(type => type.Name.EndsWith("Query"))
					.WithServiceFirstInterface());

			_container.Register(
				Classes
					.FromAssemblyNamed(shuttleScheduling)
					.Pick()
					.If(type => type.Name.EndsWith("Factory"))
					.WithServiceFirstInterface());

			_bus = ServiceBus.Create().Start();

			_container.Register(Component.For<IServiceBus>().Instance(_bus).LifestyleSingleton());

			_repository = _container.Resolve<IScheduleRepository>();
			_databaseConnectionFactory = _container.Resolve<IDatabaseConnectionFactory>();

			_thread = new Thread(ProcessSchedule);

			_thread.Start();
		}

		private void ProcessSchedule()
		{
			while (_running)
			{
				using (_databaseConnectionFactory.Create(SchedulerData.Source))
				{
					foreach (var schedule in _repository.All())
					{
						var command = schedule.Notification();

						if (command == null)
						{
							continue;
						}

						var commandSchedule = schedule;

						_bus.Send(command, c => c.WithRecipient(commandSchedule.InboxWorkQueueUri));

						_repository.Save(schedule);
					}
				}

				ThreadSleep.While(millisecondsBetweenScheduleChecks, this);
			}
		}

		public bool Active
		{
			get { return _running; }
		}
	}
}