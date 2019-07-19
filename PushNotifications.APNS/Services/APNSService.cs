using Microsoft.Extensions.Logging;
using PushNotifications.APNS.Options;
using PushNotifications.Models;
using PushNotifications.Strategies;
using System;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PushNotifications.APNS.Services
{
    public class APNSService : IAPNSSenderNotification
    {
        private readonly APNSOptions _optionsAPNS;
        private readonly ILogger<APNSService> _logger;
        private readonly AsyncLock _mutex = new AsyncLock();

        public APNSService(APNSOptions optionsAPNS, ILogger<APNSService> logger)
        {
            _optionsAPNS = optionsAPNS ?? throw new ArgumentNullException(nameof(optionsAPNS));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendNotificationAsync(Message message)
        {
            using (await _mutex.LockAsync())
            {
                TcpClient client = null;
                X509Certificate2 clientCertificate = null;
                X509Store certStore = null; ;
                int port = 2195;
                //string hostname = "gateway.sandbox.push.apple.com";
                string hostname = _optionsAPNS.APNSEndPoint;
                try
                {

                    if (!string.IsNullOrEmpty(_optionsAPNS.CertificateThumbPrint))
                    {
                        certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                        certStore.Open(OpenFlags.ReadOnly);
                        X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                                   X509FindType.FindByThumbprint,
                                                   _optionsAPNS.CertificateThumbPrint,
                                                   false);

                        clientCertificate = certCollection[0];
                    }
                    else
                    {
                        string certificatePath = _optionsAPNS.CertificatePath;
                        clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), _optionsAPNS.CertificatePassword);
                    }

                    X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

                    client = new TcpClient(AddressFamily.InterNetwork);
                    await client.ConnectAsync(hostname, port);

                    SslStream sslStream = new SslStream(
                        client.GetStream(), false,
                        new RemoteCertificateValidationCallback(ValidateServerCertificate),
                        null);


                    await sslStream.AuthenticateAsClientAsync(hostname, certificatesCollection, SslProtocols.Tls, false);

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
                    _logger.LogError($"Error sending ios notification", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending ios notification", ex);
                }
                finally
                {
                    client?.Dispose();
                    certStore?.Close();
                }
            }
        }

        private void writeIntBytesAsBigEndian(BinaryWriter writer, int value, int bytesCount)
        {
            byte[] bytes = null;
            if (bytesCount == 2)
                bytes = BitConverter.GetBytes((Int16)value);
            else if (bytesCount == 4)
                bytes = BitConverter.GetBytes((Int32)value);
            else if (bytesCount == 8)
                bytes = BitConverter.GetBytes((Int64)value);

            if (bytes != null)
            {
                Array.Reverse(bytes);
                writer.Write(bytes, 0, bytesCount);
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
