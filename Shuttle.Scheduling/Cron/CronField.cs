using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Scheduling
{
	public enum ExpressionType
	{
		Default,
		All,
		LastDayOfMonth,
		LastWeekDayOfMonth,
		LastWeekDay,
		WeekDayOccurrence,
		NearestWeekDay,
		LastDayOfWeek,
		Skipped
	}

	public abstract class CronField : ISpecification<object>
	{
		protected readonly Regex RangeExpression =
			new Regex(
				@"^(?<start>\d+)-(?<end>\d+)/(?<step>\d+)$|^(?<start>\d+)-(?<end>\d+)$|^(?<start>\d+)$|^(?<start>\d+)/(?<step>\d+)$|^(?<start>\*)/(?<step>\d+)$|^(?<start>\*)$",
				RegexOptions.IgnoreCase);

		public string Value { get; private set; }
		public ExpressionType ExpressionType { get; protected set; }

		private readonly List<ISpecification<object>> specifications = new List<ISpecification<object>>();

		protected CronField(string value)
		{
			Guard.AgainstNullOrEmptyString(value, "value");

			Value = value;
		}

		public abstract DateTime SnapForward(DateTime date);
		public abstract DateTime SnapBackward(DateTime date);

		protected string[] SplitValue()
		{
			return Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
		}

		protected void AddSpecification(ISpecification<object> specification)
		{
			Guard.AgainstNull(specification, "specification");

			specifications.Add(specification);
		}

		public virtual bool IsSatisfiedBy(object item)
		{
			return !specifications.Any() || specifications.Any(specification => specification.IsSatisfiedBy(item));
		}

		protected void DefaultParsing(int minimum, int maximum)
		{
			ExpressionType = ExpressionType.Default;

			foreach (var s in SplitValue())
			{
				var match = RangeExpression.Match(s);

				Guard.Against<CronException>(!match.Success, string.Format(SchedulingResources.CronInvalidExpression, s));

				var startValue = match.Groups["start"].Value;
				var endValue = match.Groups["end"].Value;
				var stepValue = match.Groups["step"].Value;

				var step = string.IsNullOrEmpty(stepValue)
					? 1
					: Convert.ToInt32(stepValue);

				if (startValue == "*")
				{
					AddSpecification(new RangeSpecification(0, maximum, step));

					ExpressionType = ExpressionType.All;

					return;
				}
				
				var start = Convert.ToInt32(startValue);
				var end = string.IsNullOrEmpty(endValue)
					? string.IsNullOrEmpty(stepValue)
						? start
						: maximum
					: Convert.ToInt32(endValue);

				Guard.Against<CronException>(start < minimum,
					string.Format(SchedulingResources.CronStartValueTooSmall, start, minimum));
				Guard.Against<CronException>(end < minimum,
					string.Format(SchedulingResources.CronEndValueTooSmall, end, minimum));
				Guard.Against<CronException>(start > maximum,
					string.Format(SchedulingResources.CronStartValueTooLarge, start, maximum));
				Guard.Against<CronException>(end > maximum,
					string.Format(SchedulingResources.CronEndValueTooLarge, end, maximum));

				AddSpecification(new RangeSpecification(start, end, step));
			}
		}
	}
}