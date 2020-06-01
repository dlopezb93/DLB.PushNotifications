using Microsoft.Extensions.Logging;
using PushNotifications.APNS.Strategies;
using PushNotifications.Contracts;
using PushNotifications.Enums;
using PushNotifications.Models;
using System;
using System.Threading.Tasks;

namespace PushNotifications.APNS.Services
{
    public class APNSService : ISenderNotification
    {
        private readonly ILogger<APNSService> _logger;
        private readonly ISendNotificationStrategy _sendNotificationStrategy;

        public APNSService(
                ILogger<APNSService> logger,
                ISendNotificationStrategy sendNotificationStrategy)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sendNotificationStrategy = sendNotificationStrategy ?? throw new ArgumentNullException(nameof(sendNotificationStrategy));
        }

        public Platform Platform => Platform.IOS;

        public Task SendNotificationAsync(Message message)
        {
            _logger.LogInformation($"Sending apns notification with {_sendNotificationStrategy.GetType().Name} strategy");

            return _sendNotificationStrategy.SendAsync(message);
        }
    }
}
