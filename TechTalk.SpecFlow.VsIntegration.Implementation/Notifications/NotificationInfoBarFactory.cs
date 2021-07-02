using System;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Install;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Notifications
{
    public class NotificationInfoBarFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBrowserNotificationService _browserService;
        private readonly NotificationDataStore _notificationDataStore;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;

        public NotificationInfoBarFactory(IServiceProvider serviceProvider, IBrowserNotificationService browserService, NotificationDataStore notificationDataStore, IAnalyticsTransmitter analyticsTransmitter)
        {
            _serviceProvider = serviceProvider;
            _browserService = browserService;
            _notificationDataStore = notificationDataStore;
            _analyticsTransmitter = analyticsTransmitter;
        }

        public NotificationInfoBar Create(NotificationData notification)
        {
            return new NotificationInfoBar(_serviceProvider, _browserService, _notificationDataStore, _analyticsTransmitter, notification);
        }
    }
}
