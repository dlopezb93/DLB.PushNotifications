using Microsoft.Extensions.DependencyInjection;
using PushNotifications.Contracts;
using PushNotifications.Firebase.Services;
using System;

namespace PushNotifications.Firebase.Options
{
    public class FirebaseOptionsImpl : IPushNotificationOptionsExtension
    {
        private readonly Action<FirebaseOptions> _configure;

        public FirebaseOptionsImpl(Action<FirebaseOptions> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
        }

        public void AddServices(IServiceCollection services)
        {
            var options = new FirebaseOptions();

            _configure?.Invoke(options);
            services.AddSingleton(options);

            services.AddTransient<RestSharp.RestClient>();
            services.AddSingleton<AuthFirebase>();
            services.AddScoped<ISenderNotification, FirebaseService>();
        }
    }
}
