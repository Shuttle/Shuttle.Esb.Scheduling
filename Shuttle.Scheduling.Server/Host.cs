using System;
using System.Threading;
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

		private readonly int _millisecondsBetweenScheduleChecks =
			ConfigurationItem<int>.ReadSetting("MillisecondsBetweenScheduleChecks", 5000).GetValue();

		private IServiceBus _bus;
		private IDatabaseContextFactory _databaseContextFactory;
		private IScheduleRepository _repository;

		private volatile bool _running = true;
		private Thread _thread;

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

			_container.Register(Component.For<IDatabaseContextCache>()
				.ImplementedBy<ThreadStaticDatabaseContextCache>());

			_container.Register(Component.For<IDatabaseGateway>().ImplementedBy<DatabaseGateway>());
			_container.Register(Component.For<IDatabaseContextFactory>().ImplementedBy<DatabaseContextFactory>());
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
			_databaseContextFactory = _container.Resolve<IDatabaseContextFactory>();

			_thread = new Thread(ProcessSchedule);

			_thread.Start();
		}

		public bool Active
		{
			get { return _running; }
		}

		private void ProcessSchedule()
		{
			while (_running)
			{
				using (_databaseContextFactory.Create(SchedulingData.ConnectionStringName))
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

				ThreadSleep.While(_millisecondsBetweenScheduleChecks, this);
			}
		}
	}
}