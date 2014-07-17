using System.Collections.Generic;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling.Cron
{
	public class RangeSpecification : ISpecification<object>
	{
		private readonly List<int> values = new List<int>();

		public RangeSpecification(int start, int end, int step)
		{
			Guard.Against<CronException>(end < start, InfrastructureResources.CronStartValueLargerThanEndValue);

			var value = start;

			while (value <= end)
			{
				values.Add(value);

				value += step;
			}
		}

		public RangeSpecification(int value)
			: this(value, value, 1)
		{
		}

		public bool IsSatisfiedBy(object item)
		{
			Guard.AgainstNull(item, "item");

			if (item is int)
			{
				return values.Contains((int)item);
			}

			throw new CronException(string.Format(InfrastructureResources.CronInvalidSpecificationCandidate, typeof(int).FullName, item.GetType().FullName));
		}
	}
}