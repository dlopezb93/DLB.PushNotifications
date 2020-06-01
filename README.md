# Push notifications for Android and IOS using Firebase and APNS
Push notifications for Firebase And APNS. Extension of Microsoft.Extensions.DependencyInjection

### DLB.Push.Notifications

### Installation

DLB.PushNotifications is a extensible library to use push notifications in easy way. To use this packge is neccesarry install DLB.PushNotificaions and a concret type. Firebase, APNS or both.

Install base the dependencies:

```csharp
Install-Package DLB.PushNotifications
```
Then in Startup.cs file (or anything) add next line:
```csharp
using PushNotifications.Extensions;
```

Finally in ServiceCollection class, we have avaible a extension method:
```csharp
services.AddPushNotifications(p =>
{
   
});
```
### Usage

To send a Push Notifications only need inject 'IPushNotificationsFacade' interface and use 'SendPushNotificationAsync' method  async. In this example I used a DomainEvent with DomainEventHandler, however it can be used in any situation

```csharp
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
		
```

### DLB.Push.Notifications.Firebase
### Installation

DLB.PushNotifications.Firebase is a extensible library to use firebase push notifications in easy way. To use this packge is neccesarry install DLB.PushNotificaions and DLB.PushNotifications.Firebase

Install base the dependencies:

```csharp
Install-Package DLB.PushNotifications
Install-Package DLB.PushNotifications.Firebase
```

### Configure
Then in Startup.cs file (or anything) add next lines to add firebase integration:
```csharp
using PushNotifications.Extensions;
using PushNotifications.Firebase.Extensions
```

Finally in ServiceCollection class, we have avaible a extension method:
```csharp
services.AddPushNotifications(p =>
{
        p.UseFirebase(conf =>
        {
            conf.FirebaseEndPoint = <FIREBASE_ENDPOINT>;
            conf.SenderId = <SENDER_ID>;
        });
});
```

### DLB.Push.Notifications.APNS
### Installation

Install-Package DLB.PushNotifications.APNS

### Configure
You can use DLB.PushNotifications.APNS with two ways:

- Token (p8 certifcate)
- Socket (p12/pfx certificate)

#### Token (p8 certifcate)

In Startup.cs add the following lines:

```csharp
services.AddPushNotifications(p =>
            {
                p.UseAPNS(conf =>
                {
                    conf.DeliveryType = PushNotifications.APNS.Options.DeliveryType.Token;
                    conf.TokenOptions = new PushNotifications.APNS.Options.TokenOptions()
                    {
                        APNSEndPoint = settings.APNSEndPoint,
                        BundleId = settings.APNSBundleId,
                        KeyId = settings.APNSKeyId,
                        TeamId = settings.APNSTeamId,
                        P8CertificatePath = settings.APNSCertificate,
                    };
                });
            });
```

#### Socket (p12/pfx certificate)

```csharp
services.AddPushNotifications(p =>
            {
                p.UseAPNS(conf =>
                {
                    conf.DeliveryType = PushNotifications.APNS.Options.DeliveryType.Socket;
                    conf.TokenOptions = new PushNotifications.APNS.Options.SocketOptions()
                    {
                        // Set options
                    };
                });
            });
```
