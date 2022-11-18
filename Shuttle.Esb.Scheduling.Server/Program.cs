using System;
using System.Data.Common;
using System.IO;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shuttle.Core.Data;
using Shuttle.Core.DependencyInjection;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Esb.Scheduling.Sql;
using Shuttle.Esb.Sql.Subscription;

namespace Shuttle.Esb.Scheduling.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

            var appsettingsPath = Environment.GetEnvironmentVariable("appsettingsPath");

            if (string.IsNullOrEmpty(appsettingsPath))
            {
                throw new ApplicationException("Environment variable `appsettingsPath` has not been set.");
            }

            if (!File.Exists(appsettingsPath))
            {
                throw new ApplicationException($"Environment variable `appsettingsPath` has a value of '{appsettingsPath}' that cannot be accessed/found.");
            }

            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    var configuration = new ConfigurationBuilder().AddJsonFile(appsettingsPath).Build();

                    services.AddSingleton<IConfiguration>(configuration);

                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });

                    services.FromAssembly(Assembly.Load("Shuttle.Esb.Scheduling")).Add();

                    services.AddScheduling(builder =>
                    {
                        builder.Options.ConnectionStringName = "Schedule";

                        builder.AddSqlServer();
                    });

                    services.AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("Schedule", "Microsoft.Data.SqlClient");
                        builder.AddConnectionString("Subscription", "Microsoft.Data.SqlClient");
                    });

                    services.AddServiceBus(builder =>
                    {
                        builder.Options.Subscription.ConnectionStringName = "Subscription";

                        builder.AddMessageHandlers(Assembly.Load("Shuttle.Esb.Scheduling.Handlers"));
                    });

                    services.AddSqlSubscription();

                    services.AddAzureStorageQueues(builder =>
                    {
                        builder.AddOptions("azure", new AzureStorageQueueOptions
                        {
                            ConnectionString = configuration.GetConnectionString("azure")
                        });
                    });
                })
                .Build()
                .Run();
        }
    }
}