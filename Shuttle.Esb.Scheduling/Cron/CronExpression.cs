using System;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Scheduling
{
	public class CronExpression
	{
		private CronMinute cronMinute;
		private CronHour cronHour;
		private CronDayOfMonth cronDayOfMonth;
		private CronMonth cronMonth;
		private CronDayOfWeek cronDayOfWeek;

		public string Expression { get; private set; }
		public DateTime crondate;

		public CronExpression(string expression)
			: this(expression, DateTime.Now)
		{
		}

		public CronExpression(string expression, DateTime date)
		{
			Guard.AgainstNullOrEmptyString(expression, "expression");

			Expression = expression;

			crondate = RemoveSeconds(date);

			ParseExpression(expression);
		}

		private static DateTime RemoveSeconds(DateTime date)
		{
			return date.AddSeconds(date.Second * -1);
		}

		private void ParseExpression(string expression)
		{
			var values = expression.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

			var length = values.Length;

			Guard.Against<CronException>(length != 5, string.Format(SchedulingResources.CronInvalidFieldCount, length));

			cronMinute = new CronMinute(values[0]);
			cronHour = new CronHour(values[1]);
			cronDayOfMonth = new CronDayOfMonth(values[2]);
			cronMonth = new CronMonth(values[3]);
			cronDayOfWeek = new CronDayOfWeek(values[4]);

			Guard.Against<CronException>(cronDayOfMonth.ExpressionType == ExpressionType.Skipped
			                             &&
			                             cronDayOfWeek.ExpressionType == ExpressionType.Skipped,
										 string.Format(SchedulingResources.CronNoDaysSpecified, expression));
			Guard.Against<CronException>(cronDayOfMonth.ExpressionType != ExpressionType.Skipped
			                             &&
			                             cronDayOfMonth.ExpressionType != ExpressionType.All
			                             &&
			                             cronDayOfWeek.ExpressionType != ExpressionType.Skipped
			                             &&
			                             cronDayOfWeek.ExpressionType != ExpressionType.All,
										 string.Format(SchedulingResources.CronBothDaysSpecified, expression));
		}

		public DateTime NextOccurrence()
		{
			return NextOccurrence(crondate);
		}

		public DateTime NextOccurrence(DateTime date)
		{
			crondate = SnapNextOccurrence(RemoveSeconds(date.AddMinutes(1)));
			
			var validator = SnapNextOccurrence(crondate);

			while (validator != crondate)
			{
				crondate = validator;
				validator = SnapNextOccurrence(crondate);
			}

			return crondate;
		}

		private DateTime SnapNextOccurrence(DateTime date)
		{
			var result = date;

			result = cronMinute.SnapForward(result);
			result = cronHour.SnapForward(result);
			result = cronDayOfMonth.SnapForward(result);
			result = cronMonth.SnapForward(result);
			result = cronDayOfWeek.SnapForward(result);

			return result;
		}

		public DateTime PreviousOccurrence()
		{
			return PreviousOccurrence(crondate);
		}

		public DateTime PreviousOccurrence(DateTime date)
		{
			crondate = SnapPreviousOccurrence(RemoveSeconds(date.AddMinutes(-1)));

			var validator = SnapPreviousOccurrence(crondate);

			while (validator != crondate)
			{
				crondate = validator;
				validator = SnapPreviousOccurrence(crondate);
			}

			return crondate;
		}

		private DateTime SnapPreviousOccurrence(DateTime date)
		{
			var result = date;

			result = cronMinute.SnapBackward(result);
			result = cronHour.SnapBackward(result);
			result = cronDayOfMonth.SnapBackward(result);
			result = cronMonth.SnapBackward(result);
			result = cronDayOfWeek.SnapBackward(result);

			return result;
		}
	}
}