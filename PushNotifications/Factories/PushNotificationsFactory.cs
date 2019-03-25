using PushNotifications.Contracts;
using PushNotifications.Enums;
using PushNotifications.Strategies;
using System;

namespace PushNotifications.Factories
{
    public class PushNotificationsFactory : IPushNotificationFactory
    {
        private readonly IFirebaseSenderNotification _firebaseSender;

        public PushNotificationsFactory(IFirebaseSenderNotification firebaseSender)
        {
            _firebaseSender = firebaseSender ?? throw new ArgumentNullException(nameof(firebaseSender));
        }

        public ISenderNotification GetStrategy(Platform notificationTypes)
        {
            ISenderNotification sender = null;

            if (notificationTypes.Value == Platform.Android.Value)
                sender = _firebaseSender;

            return sender;
        }
    }
}
