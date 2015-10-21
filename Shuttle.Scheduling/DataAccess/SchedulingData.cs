using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling
{
	public class SchedulingData
	{
		public static readonly string ConnectionStringName =
			ConfigurationItem<string>.ReadSetting("SchedulingConnectionStringName", "Scheduling").GetValue();
	}
}