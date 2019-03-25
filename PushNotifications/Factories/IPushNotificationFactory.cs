using PushNotifications.Contracts;
using PushNotifications.Enums;

namespace PushNotifications.Factories
{
    public interface IPushNotificationFactory
    {
        ISenderNotification GetStrategy(Platform notificationTypes);
    }
}
