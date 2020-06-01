using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PushNotifications.APNS.Helpers;
using PushNotifications.APNS.Options;
using PushNotifications.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PushNotifications.APNS.Strategies
{
    public class TokenNotificationStrategy : ISendNotificationStrategy
    {
        private readonly APNSOptions _optionsAPNS;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JwtHelper _jwtHelper;
        private readonly ILogger<SocketNotificationStrategy> _logger;

        public TokenNotificationStrategy(
                APNSOptions optionsAPNS,
                IHttpClientFactory httpClientFactory,
                JwtHelper jwtHelper,
                ILogger<SocketNotificationStrategy> logger)
        {
            _optionsAPNS = optionsAPNS ?? throw new ArgumentNullException(nameof(optionsAPNS));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jwtHelper = jwtHelper ?? throw new ArgumentNullException(nameof(jwtHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(Message message)
        {
            var options = _optionsAPNS.TokenOptions;

            ValidateOptions();

            try
            {
                var data = await System.IO.File.ReadAllTextAsync(options.P8CertificatePath);
                var list = data.Split('\n').ToList();
                var prk = list.Where((s, i) => i != 0 && i != list.Count - 1).Aggregate((agg, s) => agg + s);
                var key = new ECDsaCng(CngKey.Import(Convert.FromBase64String(prk), CngKeyBlobFormat.Pkcs8PrivateBlob));
                var token = _jwtHelper.GetToken(key, options.KeyId, options.TeamId, message.DeviceToken);
                var url = ApnsHelper.BuildEndpoint(options.APNSEndPoint, message.DeviceToken);
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Headers.TryAddWithoutValidation("apns-push-type", "alert"); // or background
                request.Headers.TryAddWithoutValidation("apns-id", Guid.NewGuid().ToString("D"));
                request.Headers.TryAddWithoutValidation("apns-expiration", Convert.ToString(0));
                request.Headers.TryAddWithoutValidation("apns-priority", Convert.ToString(10));
                request.Headers.TryAddWithoutValidation("apns-topic", options.BundleId);
                request.Version = HttpVersion.Version20;

                var body = GetMessage(message);

                using (var stringContent = new StringContent(body, Encoding.UTF8, "application/json"))
                {
                    //Set Body
                    request.Content = stringContent;

                    await Task.Delay(TimeSpan.FromSeconds(3));

                    HttpClient client = _httpClientFactory.CreateClient();
                    HttpResponseMessage resp = await client.SendAsync(request);

                    if (resp != null)
                    {
                        string apnsResponseString = await resp.Content.ReadAsStringAsync();
                        string apnsMessage = string.IsNullOrEmpty(apnsResponseString) ? "OK" : apnsResponseString;

                        _logger.LogInformation($"APNS Response: {apnsMessage}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Exception in http request");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void ValidateOptions()
        {
            var options = _optionsAPNS.TokenOptions;

            if (!System.IO.File.Exists(options.P8CertificatePath))
            {
                throw new FileNotFoundException($"File '{options.P8CertificatePath}' not found");
            }            

            if (string.IsNullOrWhiteSpace(options.TeamId))
            {
                throw new ArgumentException("TeamId cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(options.KeyId))
            {
                throw new ArgumentException("KeyId cannot be null or empty");
            }

            if (string.IsNullOrWhiteSpace(options.BundleId))
            {
                throw new ArgumentException("BundleId cannot be null or empty");
            }
        }

        private string GetMessage(Message message)
        {
            var body = JsonConvert.SerializeObject(new
            {
                aps = new
                {
                    alert = new
                    {
                        title = message.Notification.Title,
                        body = message.Notification.Description,
                        time = DateTime.Now.ToString()
                    },
                    badge = 1,
                    sound = "default"
                },
                //acme2 = new string[] { "bang", "whiz" }
            });

            return body;
        }
    }
}
