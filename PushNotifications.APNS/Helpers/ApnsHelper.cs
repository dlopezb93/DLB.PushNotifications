using System;

namespace PushNotifications.APNS.Helpers
{
    public class ApnsHelper
    {
        public static string BuildEndpoint(string endpoint, string deviceToken)
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                throw new ArgumentException("Endpoint cannot be empty.");
            }

            endpoint = !endpoint.EndsWith("/") ? $"{endpoint}/" : endpoint;

            return $"{endpoint}3/device/{deviceToken}";
        }
    }
}
