using System;
using System.Collections.Generic;
using System.Text;

namespace PushNotifications.APNS.Options
{
    public class SocketOptions
    {
        public string APNSEndPoint { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassword { get; set; }

        public string CertificateThumbPrint { get; set; }
    }
}
