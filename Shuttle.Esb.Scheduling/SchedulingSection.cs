using System;
using System.Configuration;
using Shuttle.Core.Configuration;

namespace Shuttle.Esb.Scheduling
{
    public class SchedulingSection : ConfigurationSection
    {
        [ConfigurationProperty("connectionStringName", IsRequired = false, DefaultValue = "Scheduling")]
        public string ConnectionStringName => (string) this["connectionStringName"];

        [ConfigurationProperty("secondsBetweenScheduleChecks", IsRequired = false, DefaultValue = 15)]
        public int SecondsBetweenScheduleChecks => (int) this["secondsBetweenScheduleChecks"];

        public static ISchedulingConfiguration Configuration()
        {
            var section = ConfigurationSectionProvider.Open<SchedulingSection>("shuttle", "scheduling");
            var configuration = new SchedulingConfiguration();

            var connectionStringName = "Scheduling";

            if (section != null)
            {
                connectionStringName = section.ConnectionStringName;
                configuration.SecondsBetweenScheduleChecks = section.SecondsBetweenScheduleChecks;
            }

            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings == null)
            {
                throw new ApplicationException($"Could not find a connection string with name '{connectionStringName}'.");
            }

            configuration.ConnectionString = settings.ConnectionString;
            configuration.ProviderName = settings.ProviderName;

            return configuration;
        }
    }
}