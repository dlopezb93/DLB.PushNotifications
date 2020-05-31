using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PushNotifications.Contracts;
using PushNotifications.Enums;
using PushNotifications.Firebase.Options;
using PushNotifications.Models;
using RestSharp;
using System.Threading.Tasks;

namespace PushNotifications.Firebase.Services
{
    public class FirebaseService : BaseService, ISenderNotification
    {
        private readonly FirebaseOptions _options;
        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(FirebaseOptions options, 
                               RestClient restClient, 
                               AuthFirebase auth, 
                               ILogger<FirebaseService> logger) : base(options,restClient, auth)
        {
            _options = options ?? throw new System.ArgumentNullException(nameof(options));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public Platform Platform => Platform.Android;

        public Task SendNotificationAsync(Message message)
        {
            IRestRequest request = new RestRequest("/fcm/send", Method.POST ,DataFormat.Json);
            var data = new
            {
                to = message.DeviceToken,
                notification = new
                {
                    title = message.Notification.Title,
                    body = message.Notification.Description                
                },
                priority = "high"
            };

            request.AddBody(JsonConvert.SerializeObject(data));

            return ExecuteAsync<object>(request);
        }
    }
}
