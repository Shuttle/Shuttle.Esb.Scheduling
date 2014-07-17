using System;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shuttle.ESB.Modules;
using log4net;
using log4net.Config;
using Shuttle.Core.Data;
using Shuttle.Core.Domain;
using Shuttle.Core.Domain.Castle;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Core;
using Shuttle.ESB.SqlServer;

namespace Shuttle.Scheduling.Server
{
	public class ServiceBusHost : IHost, IDisposable, IActiveState
	{
		private readonly WindsorContainer container = new WindsorContainer();

		private IServiceBus bus;

		private volatile bool running = true;
		private Thread thread;
		private IDatabaseConnectionFactory databaseConnectionFactory;
		private IScheduleRepository repository;

		private readonly int millisecondsBetweenScheduleChecks = ConfigurationItem<int>.ReadSetting("MillisecondsBetweenScheduleChecks", 5000).GetValue();

		public void Dispose()
		{
			running = false;

			bus.Dispose();

			LogManager.Shutdown();
		}

		public void Start()
		{
			XmlConfigurator.Configure();

			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (Program))));

            new ConnectionStringService().Approve();

			container.Register(Component.For<IDatabaseConnectionCache>()
										.ImplementedBy<ThreadStaticDatabaseConnectionCache>());

			container.Register(Component.For<IDbConnectionConfiguration>()
										.ImplementedBy<DbConnectionConfiguration>());

			container.Register(Component.For<IDatabaseGateway>().ImplementedBy<DatabaseGateway>());
			container.Register(Component.For<IDatabaseConnectionFactory>().ImplementedBy<DatabaseConnectionFactory>());
			container.Register(Component.For(typeof(IDataRepository<>)).ImplementedBy(typeof(DataRepository<>)));

			container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Core.Data")
					.Pick()
					.If(type => type.Name.EndsWith("Factory"))
					.Configure(configurer => configurer.Named(configurer.Implementation.Name.ToLower()))
					.WithService.Select((type, basetype) => new[] { type.InterfaceMatching(RegexPatterns.EndsWith("Factory")) }));

			container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Scheduling")
					.BasedOn(typeof(IDataRowMapper<>))
					.WithServiceFirstInterface());

			container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Scheduling")
					.Pick()
					.If(type => type.Name.EndsWith("Repository"))
					.WithServiceFirstInterface());

			container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Scheduling")
					.Pick()
					.If(type => type.Name.EndsWith("Query"))
					.WithServiceFirstInterface());

			container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Scheduling")
					.Pick()
					.If(type => type.Name.EndsWith("QueryFactory"))
					.WithServiceFirstInterface());

			container.Register(
				Classes
					.FromAssemblyNamed("Shuttle.Scheduling")
					.Pick()
					.If(type => type.Name.EndsWith("DomainHandler"))
					.WithServiceFirstInterface());

            DomainEvents.Assign(new DomainEventDispatcher(container));

			bus = ServiceBus
				.Create()
				.SubscriptionManager(SubscriptionManager.Default())
				.AddModule(new ActiveTimeRangeModule())
				.Start();

			container.Register(Component.For<IServiceBus>().Instance(bus));

			repository = container.Resolve<IScheduleRepository>();
			databaseConnectionFactory = container.Resolve<IDatabaseConnectionFactory>();

			thread = new Thread(ProcessSchedule);

			thread.Start();
		}

		private void ProcessSchedule()
		{
			while (running)
			{
				using (databaseConnectionFactory.Create(SchedulerData.Source))
				{
					repository.All().ForEach(schedule => schedule.CheckNotification());
				}

				ThreadSleep.While(millisecondsBetweenScheduleChecks, this);
			}
		}

		public bool Active
		{
			get { return running; }
		}
	}
}