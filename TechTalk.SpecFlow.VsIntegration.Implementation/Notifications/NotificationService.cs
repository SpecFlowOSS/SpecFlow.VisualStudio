using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow.IdeIntegration.Install;
using Task = System.Threading.Tasks.Task;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Notifications
{
    public class NotificationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBrowserNotificationService _browserService;
        private readonly NotificationDataStore _notificationDataStore;

        public NotificationService(IServiceProvider serviceProvider, IBrowserNotificationService browserService, NotificationDataStore notificationDataStore)
        {
            _serviceProvider = serviceProvider;
            _browserService = browserService;
            this._notificationDataStore = notificationDataStore;
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
                var notification = await GetNotification();

                if (notification != null && !_notificationDataStore.IsDismissed(notification))
                    await Notify(notification);
            }
            catch
            {
                // ignored
            }
        }


        
        private static async Task<NotificationData> GetNotification()
        {
            //TODO: POC implementation, should call notification service
            var httpClient = new HttpClient();
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var result = await httpClient.GetAsync("https://specflow.org/tools/releases/");
            result.EnsureSuccessStatusCode();
            var content = await result.Content.ReadAsStringAsync();

            var regex = new Regex(@"<h2 class=""[^""]*"">([^>]*)</h2>");
            var match = regex.Match(content);
            var text = match.Groups[1].Value;
            return new NotificationData
            {
                Id = text,
                Message = text,
                LinkText = "Learn more",
                LinkUrl = "https://specflow.org/tools/releases/"
            };
        }

        private async Task Notify(NotificationData notification)
        {
            //Showing notification with a slight delay to prove that this thread does not block Visual Studio
            await Task.Delay(TimeSpan.FromSeconds(20));

            var infoBar = new NotificationInfoBar(_serviceProvider, _browserService, _notificationDataStore, notification);
            await infoBar.ShowInfoBar();
        }
    }
}
