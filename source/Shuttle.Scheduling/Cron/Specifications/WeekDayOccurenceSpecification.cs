using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling.Cron
{
	public class WeekDayOccurenceSpecification : ISpecification<object>
	{
		private readonly int weekDay;
		private readonly int occurrence;

		public WeekDayOccurenceSpecification(int weekDay, int occurrence)
		{
			this.weekDay = weekDay;
			this.occurrence = occurrence;
		}

		public bool IsSatisfiedBy(object item)
		{
			Guard.AgainstNull(item, "item");

			if (item is DateTime)
			{
				var date = (DateTime)item;
				var day = ((int)date.DayOfWeek) + 1;

				return (day == weekDay && occurrence == ((date.Day / 7) + 1));
			}

			throw new CronException(string.Format(InfrastructureResources.CronInvalidSpecificationCandidate, typeof(int).FullName, item.GetType().FullName));
		}
	}
}