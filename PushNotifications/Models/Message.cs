using PushNotifications.Enums;

namespace PushNotifications.Models
{
    public class Message
    {
        public Message(Platform platform, string deviceToken)
        {
            Platform = platform;
            DeviceToken = deviceToken;
        }

        public string DeviceToken { get; }

        public Platform Platform { get; }

        public Notification Notification { get; set; }
    }
}
