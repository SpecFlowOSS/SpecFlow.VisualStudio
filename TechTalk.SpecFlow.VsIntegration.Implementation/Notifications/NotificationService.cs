using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Notifications
{
    public class NotificationService
    {
        private readonly NotificationDataStore _notificationDataStore;
        private readonly NotificationInfoBarFactory _notificationInfoBarFactory;

        private const string DefaultApiUrl = "https://notifications.specflow.org/api/notifications/visualstudio";
        private const string SpecFlowNotificationUnpublishedEnvironmentVariable = "SPECFLOW_NOTIFICATION_UNPUBLISHED";

        public NotificationService(NotificationDataStore notificationDataStore, NotificationInfoBarFactory notificationInfoBarFactory)
        {
            _notificationDataStore = notificationDataStore;
            _notificationInfoBarFactory = notificationInfoBarFactory;
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
            var infoBar = _notificationInfoBarFactory.Create(notification);
            await infoBar.ShowInfoBar();
        }
    }
}
