namespace PushNotifications.Models
{
    public class Notification
    {
        public Notification(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public object Data { get; set; }
    }
}
