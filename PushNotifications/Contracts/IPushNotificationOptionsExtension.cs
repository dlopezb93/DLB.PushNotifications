using Microsoft.Extensions.DependencyInjection;

namespace PushNotifications.Contracts
{
    public interface IPushNotificationOptionsExtension
    {
        /// <summary>
        /// Registered child service.
        /// </summary>
        /// <param name="services">add service to the <see cref="IServiceCollection" /></param>
        void AddServices(IServiceCollection services);
    }
}
