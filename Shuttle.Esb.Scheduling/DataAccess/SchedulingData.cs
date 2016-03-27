using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Scheduling
{
	public class SchedulingData
	{
		public static readonly string ConnectionStringName =
			ConfigurationItem<string>.ReadSetting("SchedulingConnectionStringName", "Scheduling").GetValue();
	}
}