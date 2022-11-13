using System;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.Scheduling
{
    public class SchedulingBuilder
    {
        private SchedulingOptions _schedulingOptions = new SchedulingOptions();

        public IServiceCollection Services { get; }

        public SchedulingBuilder(IServiceCollection services)
        {
            Guard.AgainstNull(services, nameof(services));

            Services = services;
        }

        public SchedulingOptions Options
        {
            get => _schedulingOptions;
            set => _schedulingOptions = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}