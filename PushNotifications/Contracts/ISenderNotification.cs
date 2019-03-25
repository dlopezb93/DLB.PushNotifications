using PushNotifications.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PushNotifications.Contracts
{
    public interface ISenderNotification
    {
        Task SendNotificationAsync(Message message);
    }
}
