# DLB.PushNotifications
Push notifications for Firebase And APNS. Extension of Microsoft.Extensions.DependencyInjection

### DLB.Push.Notifications

### Installation

DLB.PushNotifications is a extensible library to use push notifications in easy way. To use this packge is neccesarry install DLB.PushNotificaions and a concret type. Firebase, APNS or both.

Install base the dependencies:

```sh
Install-Package DLB.PushNotifications -Version 1.0.0
```
Then in Startup.cs file (or anything) add next line:
```sh
using PushNotifications.Extensions;
```

Finally in ServiceCollection class, we have avaible a extension method:
```sh
services.AddPushNotifications(p =>
{
   
});
```


### DLB.Push.Notifications.Firebase
### Installation

DLB.PushNotifications.Firebase is a extensible library to use firebase push notifications in easy way. To use this packge is neccesarry install DLB.PushNotificaions and DLB.PushNotifications.Firebase

Install base the dependencies:

```sh
Install-Package DLB.PushNotifications -Version 1.0.0
Install-Package DLB.PushNotifications.Firebase -Version 1.0.0
```

### Usage
Then in Startup.cs file (or anything) add next lines to add firebase integration:
```sh
using PushNotifications.Extensions;
using PushNotifications.Firebase.Extensions
```

Finally in ServiceCollection class, we have avaible a extension method:
```sh
services.AddPushNotifications(p =>
{
        p.UseFirebase(conf =>
        {
            conf.FirebaseEndPoint = <FIREBASE_ENDPOINT>;
            conf.SenderId = <SENDER_ID>;
        });
});
```

To send a Push Notifications only need inject 'IPushNotificationsFacade' interface and use 'SendPushNotificationAsync' method  async.

```sh
public DomainEventHandler(ILogger<UserNotificationInsertedDomainEventHandler> logger,
                          IPushNotificationsFacade pushNotifications) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pushNotifications = pushNotifications ?? 
                                throw new ArgumentNullException(nameof(pushNotifications));
        }
        
 public async Task Handle(DomainEvent notification)
        {
            var msg = new Message(Platform.Android, notification.PushToken.DeviceToken);
            var payload = new Notification(description.Title, description.Description);

            msg.Notification = payload;
            await _pushNotifications.SendPushNotificationAsync(msg);
        }
