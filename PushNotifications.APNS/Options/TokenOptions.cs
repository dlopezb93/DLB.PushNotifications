using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifications.APNS.Options
{
    public class TokenOptions
    {
        public string APNSEndPoint { get; set; }

        public string TeamId { get; set; }

        public string KeyId { get; set; }

        public string P8CertificatePath { get; set; }

        public string BundleId { get; set; }
    }
}
