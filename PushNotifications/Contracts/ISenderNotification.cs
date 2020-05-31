using PushNotifications.Enums;
using PushNotifications.Models;
using System.Threading.Tasks;

namespace PushNotifications.Contracts
{
    public interface ISenderNotification
    {
        Task SendNotificationAsync(Message message);

        Platform Platform { get; }
    }
}
