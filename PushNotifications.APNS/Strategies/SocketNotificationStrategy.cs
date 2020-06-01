using Microsoft.Extensions.Logging;
using PushNotifications.APNS.Options;
using PushNotifications.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PushNotifications.APNS.Strategies
{
    public class SocketNotificationStrategy : ISendNotificationStrategy
    {
        private readonly APNSOptions _optionsAPNS;
        private readonly ILogger<SocketNotificationStrategy> _logger;
        private readonly AsyncLock _mutex = new AsyncLock();

        public SocketNotificationStrategy(APNSOptions optionsAPNS, ILogger<SocketNotificationStrategy> logger)
        {
            _optionsAPNS = optionsAPNS ?? throw new ArgumentNullException(nameof(optionsAPNS));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(Message message)
        {
            var options = _optionsAPNS.SocketOptions;

            using (await _mutex.LockAsync())
            {
                TcpClient client = null;
                X509Certificate2 clientCertificate = null;
                X509Store certStore = null; ;
                int port = 2195;
                //string hostname = "gateway.sandbox.push.apple.com";
                try
                {

                    if (!string.IsNullOrEmpty(options.CertificateThumbPrint))
                    {
                        certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                        certStore.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                                   X509FindType.FindByThumbprint,
                                                   options.CertificateThumbPrint,
                                                   false);

                        clientCertificate = certCollection[0];
                    }
                    else
                    {
                        string certificatePath = options.CertificatePath;
                        clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), options.CertificatePassword);
                    }

                    X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

                    client = new TcpClient(AddressFamily.InterNetwork);
                    await client.ConnectAsync(options.APNSEndPoint, port);

                    SslStream sslStream = new SslStream(
                        client.GetStream(), false,
                        new RemoteCertificateValidationCallback(ValidateServerCertificate),
                        null);


                    await sslStream.AuthenticateAsClientAsync(options.APNSEndPoint, certificatesCollection, SslProtocols.Tls, false);

                    MemoryStream memoryStream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(memoryStream);
                    writer.Write((byte)0);
                    writer.Write((byte)0);
                    writer.Write((byte)32);

                    writer.Write(HexStringToByteArray(message.DeviceToken));

                    string payload = "{\"aps\":{\"alert\":\"" + message.Notification.Description + "\",\"badge\":0,\"sound\":\"default\"}}";

                    writer.Write((byte)0);
                    writer.Write((byte)payload.Length);
                    //byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                    byte[] b1 = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(payload);
                    writer.Write(b1);
                    writer.Flush();
                    byte[] array = memoryStream.ToArray();
                    sslStream.Write(array);
                    sslStream.Flush();


                }
                catch (AuthenticationException ex)
                {
                    _logger.LogError(ex, $"Error sending ios notification");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending ios notification");
                }
                finally
                {
                    client?.Dispose();
                    certStore?.Close();
                }
            }
        }

        #region Helper methods
        private byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
        #endregion
    }
}
