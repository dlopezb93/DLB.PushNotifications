using PushNotifications.APNS.Options;
using PushNotifications.Models;
using System.Threading.Tasks;

namespace PushNotifications.APNS.Strategies
{
    public interface ISendNotificationStrategy
    {
        Task SendAsync(Message message);
    }
}
