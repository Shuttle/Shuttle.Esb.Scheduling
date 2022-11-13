using Shuttle.Core.Contract;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Shuttle.Esb.Scheduling
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScheduling(this IServiceCollection services, Action<SchedulingBuilder> builder = null)
        {
            Guard.AgainstNull(services, nameof(services));

            var schedulingBuilder = new SchedulingBuilder(services);

            builder?.Invoke(schedulingBuilder);

            services.TryAddSingleton<IValidateOptions<SchedulingOptions>, SchedulingOptionsValidator>();
            services.AddSingleton<IScheduleQueryFactory, ScheduleQueryFactory>();
            services.AddSingleton<IScheduleQuery, ScheduleQuery>();
            services.AddSingleton<IScheduleRepository, ScheduleRepository>();

            services.AddOptions<SchedulingOptions>().Configure(options =>
            {
                options.ConnectionStringName =
                    schedulingBuilder.Options.ConnectionStringName;
                options.ScheduleProcessingInterval =
                    schedulingBuilder.Options.ScheduleProcessingInterval;
            });

            return services;
        }
    }
}