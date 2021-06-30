using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TechTalk.SpecFlow.IdeIntegration.Analytics;
using TechTalk.SpecFlow.IdeIntegration.Install;
using Task = System.Threading.Tasks.Task;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Notifications
{
    public class NotificationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBrowserNotificationService _browserService;
        private readonly NotificationDataStore _notificationDataStore;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;

        public NotificationService(IServiceProvider serviceProvider, IBrowserNotificationService browserService, NotificationDataStore notificationDataStore, IAnalyticsTransmitter analyticsTransmitter)
        {
            _serviceProvider = serviceProvider;
            _browserService = browserService;
            _notificationDataStore = notificationDataStore;
            _analyticsTransmitter = analyticsTransmitter;
        }


        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            //Fire and forget no await
#pragma warning disable 4014
            Task.Run(CheckAndNotifyAsync, cancellationToken);
#pragma warning restore 4014
            return Task.CompletedTask;
        }

        private async Task CheckAndNotifyAsync()
        {
            try
            {
                var notification = await GetNotificationAsync();

                if (notification != null && !_notificationDataStore.IsDismissed(notification))
                    await NotifyAsync(notification);
            }
            catch
            {
                // ignored
            }
        }

        private const string DefaultApiUrl = "https://notifications.specflow.org/api/notifications/visualstudio";
        private const string SpecFlowNotificationUnpublishedEnvironmentVariable = "SPECFLOW_NOTIFICATION_UNPUBLISHED";

        private static string GetApiUrl()
        {
            return Environment.GetEnvironmentVariable(SpecFlowNotificationUnpublishedEnvironmentVariable) != "1" ?
                    DefaultApiUrl : $"{DefaultApiUrl}/unpublished";
        }

        private static async Task<NotificationData> GetNotificationAsync()
        {
            var httpClient = new HttpClient();
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var result = await httpClient.GetAsync(GetApiUrl());
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<NotificationData>(content);
        }

        private async Task NotifyAsync(NotificationData notification)
        {
            //Showing notification with a slight delay to prove that this thread does not block Visual Studio
            await Task.Delay(TimeSpan.FromSeconds(20));

            var infoBar = new NotificationInfoBar(_serviceProvider, _browserService, _notificationDataStore, _analyticsTransmitter, notification);
            await infoBar.ShowInfoBar();
        }
    }
}
