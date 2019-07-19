using PushNotifications.APNS.Options;
using PushNotifications;
using System;

namespace PushNotifications.APNS.Extensions
{
    public static class APNSPushNotificationExtension
    {
        public static PushNotificationOptions UseAPNS(this PushNotificationOptions options, Action<APNSOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            options.RegisterExtension(new APNSOptionsImpl(configure));

            return options;
        }
    }
}
