using System;
using System.IO;
using System.Text;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Notifications
{
    public class NotificationDataStore
    {
        public static readonly string NotificationFilePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\SpecFlow\notification_vs");

        public bool IsDismissed(NotificationData notification)
        {
            try
            {
                var text = File.ReadAllText(NotificationFilePath, Encoding.UTF8);
                if (text == notification.Id) return true;
            }
            catch
            {
                // No errors
            }

            return false;
        }

        public void SetDismissed(NotificationData notification)
        {
            try
            {
                File.WriteAllText(NotificationFilePath, notification.Id, Encoding.UTF8);
            }
            catch
            {
                // No errors
            }
        }
    }
}
