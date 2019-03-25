namespace PushNotifications.Enums
{
    public sealed class Platform
    {
        public static Platform Android = new Platform(1);

        public static Platform IOS = new Platform(2);


        private Platform(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
