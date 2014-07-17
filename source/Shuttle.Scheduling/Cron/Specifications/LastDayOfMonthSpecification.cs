using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling.Cron
{
	public class LastDayOfMonthSpecification : ISpecification<object>
	{
		public bool IsSatisfiedBy(object item)
		{
			Guard.AgainstNull(item, "item");

			if (item is DateTime)
			{
				var date = (DateTime)item;

				return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
			}

			throw new CronException(string.Format(InfrastructureResources.CronInvalidSpecificationCandidate, typeof(int).FullName, item.GetType().FullName));
		}
	}
}