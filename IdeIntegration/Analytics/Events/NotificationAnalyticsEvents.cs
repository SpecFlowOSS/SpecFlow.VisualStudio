using System;

namespace TechTalk.SpecFlow.IdeIntegration.Analytics.Events
{
    public abstract class NotificationAnalyticsEventBase : AnalyticsEventBase
    {
        public string NotificationId { get; }

        protected NotificationAnalyticsEventBase(string ide, DateTime utcDate, string userId, string notificationId) : base(ide, utcDate, userId)
        {
            NotificationId = notificationId;
        }
    }

    public class NotificationShownAnalyticsEvent : NotificationAnalyticsEventBase
    {
        public NotificationShownAnalyticsEvent(string ide, DateTime utcDate, string userId, string notificationId) : base(ide, utcDate, userId, notificationId)
        {
        }

        public override string EventName => "Notification shown";
    }

    public class NotificationLinkOpenedAnalyticsEvent : NotificationAnalyticsEventBase
    {
        public NotificationLinkOpenedAnalyticsEvent(string ide, DateTime utcDate, string userId, string notificationId) : base(ide, utcDate, userId, notificationId)
        {
        }

        public override string EventName => "Notification link opened";
    }
}
