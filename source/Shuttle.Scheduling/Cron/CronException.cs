using System;

namespace Shuttle.Scheduling.Cron
{
	public class CronException : Exception
	{
		public CronException(string message) : base(message)
		{
		}
	}
}