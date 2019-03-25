using PushNotifications.Factories;
using PushNotifications.Models;
using System;
using System.Threading.Tasks;

namespace PushNotifications.Facades
{
    public class PushNotificationFacade : IPushNotificationsFacade
    {
        private readonly IPushNotificationFactory _notificationFactory;

        public PushNotificationFacade(IPushNotificationFactory notificationFactory)
        {
            _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
        }

        public async Task SendPushNotificationAsync(Message message)
        {
            var sender = _notificationFactory.GetStrategy(message.Platform);

            if (sender != null)
                await sender.SendNotificationAsync(message);
        }
    }
}
