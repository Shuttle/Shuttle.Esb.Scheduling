using System.Configuration;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Scheduling
{
    public class SchedulingSection : ConfigurationSection
    {
        [ConfigurationProperty("connectionStringName", IsRequired = false, DefaultValue = "Scheduling")]
        public string ConnectionStringName
        {
            get { return (string)this["connectionStringName"]; }
        }

        [ConfigurationProperty("millisecondsBetweenScheduleChecks", IsRequired = false, DefaultValue = 5000)]
        public int MillisecondsBetweenScheduleChecks
        {
            get { return (int)this["millisecondsBetweenScheduleChecks"]; }
        }

        public static SchedulingConfiguration Configuration()
        {
            var section = ConfigurationSectionProvider.Open<SchedulingSection>("shuttle", "scheduling");
            var configuration = new SchedulingConfiguration();

            var connectionStringName = "Scheduling";

            if (section != null)
            {
                connectionStringName = section.ConnectionStringName;
                configuration.MillisecondsBetweenScheduleChecks = section.MillisecondsBetweenScheduleChecks;
            }

            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings != null)
            {
                configuration.ConnectionString = settings.ConnectionString;
                configuration.ProviderName = settings.ProviderName;
            }

            return configuration;
        }
    }
}