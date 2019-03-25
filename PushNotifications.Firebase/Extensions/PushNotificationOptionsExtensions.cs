using PushNotifications.Firebase.Options;
using System;

namespace PushNotifications.Firebase.Extensions
{
    public static class PushNotificationOptionsExtensions
    {
        public static PushNotificationOptions UseFirebase(this PushNotificationOptions options, Action<FirebaseOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            options.RegisterExtension(new FirebaseOptionsImpl(configure));

            return options;
        }
    }
}
