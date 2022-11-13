using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Scheduling
{
    public class SchedulingOptionsValidator : IValidateOptions<SchedulingOptions>
    {
        public ValidateOptionsResult Validate(string name, SchedulingOptions options)
        {
            Guard.AgainstNull(options, nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionStringName))
            {
                return ValidateOptionsResult.Fail(Resources.ConnectionStringNameException);
            }

            return ValidateOptionsResult.Success;
        }
    }
}