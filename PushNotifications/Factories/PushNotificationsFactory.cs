using PushNotifications.Contracts;
using PushNotifications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PushNotifications.Factories
{
    public class PushNotificationsFactory : IPushNotificationFactory
    {
        private readonly IEnumerable<ISenderNotification> _senderNotifications;

        public PushNotificationsFactory(IEnumerable<ISenderNotification> senderNotifications)
        {
            _senderNotifications = senderNotifications ?? throw new ArgumentNullException(nameof(senderNotifications));
        }

        public ISenderNotification GetStrategy(Platform notificationTypes)
        {
            ISenderNotification sender = _senderNotifications.FirstOrDefault(p => p.Platform == notificationTypes);

            if (sender == null)
            {
                throw new InvalidOperationException($"There aren't any strategy registered for platorm '{notificationTypes}'. Please use UseApns() or UseFirebase()");
            }

            return sender;
        }
    }
}
