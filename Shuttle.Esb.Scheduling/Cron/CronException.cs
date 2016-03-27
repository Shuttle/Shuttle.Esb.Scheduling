using System;

namespace Shuttle.Esb.Scheduling
{
	public class CronException : Exception
	{
		public CronException(string message) : base(message)
		{
		}
	}
}