using PushNotifications.Models;
using System.Threading.Tasks;

namespace PushNotifications.Facades
{
    public interface IPushNotificationsFacade
    {
        Task SendPushNotificationAsync(Message message);
    }
}
