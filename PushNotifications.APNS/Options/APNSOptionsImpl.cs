using Microsoft.Extensions.DependencyInjection;
using PushNotifications.APNS.Services;
using PushNotifications.Contracts;
using PushNotifications.Strategies;
using System;

namespace PushNotifications.APNS.Options
{
    public class APNSOptionsImpl : IPushNotificationOptionsExtension
    {
        private readonly Action<APNSOptions> _configure;

        public APNSOptionsImpl(Action<APNSOptions> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public void AddServices(IServiceCollection services)
        {
            var options = new APNSOptions();

            _configure?.Invoke(options);

            services.AddSingleton(options);
            services.AddSingleton<IAPNSSenderNotification, APNSService>();
        }
    }
}