using Microsoft.Extensions.DependencyInjection;
using PushNotifications.APNS.Helpers;
using PushNotifications.APNS.Services;
using PushNotifications.APNS.Strategies;
using PushNotifications.Contracts;
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

            if (options.DeliveryType == DeliveryType.Token)
            {
                services.AddScoped<ISendNotificationStrategy, TokenNotificationStrategy>();
            }
            else
            {
                services.AddScoped<ISendNotificationStrategy, SocketNotificationStrategy>();
            }

            services.AddHttpClient();
            services.AddSingleton(options);
            services.AddScoped<ISenderNotification, APNSService>();
            services.AddScoped<JwtHelper>();
        }
    }
}