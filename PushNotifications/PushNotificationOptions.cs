using PushNotifications.Contracts;
using System;
using System.Collections.Generic;

namespace PushNotifications
{
    public class PushNotificationOptions
    {
        public PushNotificationOptions()
        {
            Extensions = new List<IPushNotificationOptionsExtension>();
        }

        internal IList<IPushNotificationOptionsExtension> Extensions { get; }

        /// <summary>
        /// Registers an extension that will be executed when building services.
        /// </summary>
        /// <param name="extension"></param>
        public void RegisterExtension(IPushNotificationOptionsExtension extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Extensions.Add(extension);
        }
    }    
}
