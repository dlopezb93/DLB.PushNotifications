namespace PushNotifications.APNS.Options
{
    public class APNSOptions
    {
        public DeliveryType DeliveryType { get; set; }

        public TokenOptions TokenOptions { get; set; }    
        
        public SocketOptions SocketOptions { get; set; }        
    }
}
