using PushNotifications.Facades;
using PushNotifications.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace PushNotifications.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPushNotifications(this IServiceCollection services, 
                                                Action<PushNotificationOptions> setupAction)
        {
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            //Options and extension service
            var options = new PushNotificationOptions();

            setupAction(options);

            foreach (var serviceExtension in options.Extensions)
            {
                serviceExtension.AddServices(services);
            }

            services.AddSingleton<IPushNotificationFactory, PushNotificationsFactory>();
            services.AddSingleton<IPushNotificationsFacade, PushNotificationFacade>();
            services.AddSingleton(options);
        }
    }
}
