using System;

namespace Shuttle.Scheduling
{
	public class CronException : Exception
	{
		public CronException(string message) : base(message)
		{
		}
	}
}