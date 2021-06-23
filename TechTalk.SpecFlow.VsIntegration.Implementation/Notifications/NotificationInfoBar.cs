using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TechTalk.SpecFlow.IdeIntegration.Install;
using Task = System.Threading.Tasks.Task;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.Notifications
{
    public class NotificationInfoBar : IVsInfoBarUIEvents
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGuidanceNotificationService _notificationService;
        private uint _cookie;

        public NotificationInfoBar(IServiceProvider serviceProvider, IGuidanceNotificationService notificationService)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            infoBarUIElement.Unadvise(_cookie);
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string url = (string)actionItem.ActionContext;

            _notificationService.ShowPage(url);
        }

        public async Task ShowInfoBar(string message, string linkText, string linkUrl)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var shell = _serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (shell != null)
            {
                shell.GetProperty((int)__VSSPROPID7.VSSPROPID_MainWindowInfoBarHost, out var obj);
                var host = (IVsInfoBarHost)obj;

                if (host == null)
                {
                    return;
                }

                InfoBarTextSpan text = new InfoBarTextSpan(message);
                var actionItems = new List<InfoBarActionItem>();
                if (!string.IsNullOrWhiteSpace(linkText) && !string.IsNullOrWhiteSpace(linkUrl))
                {
                    actionItems.Add(new InfoBarHyperlink(linkText, linkUrl));
                }
                InfoBarModel infoBarModel = new InfoBarModel(
                    new[] { text },
                    actionItems,
                    KnownMonikers.StatusInformation,
                    isCloseButtonVisible: true);

                var factory = _serviceProvider.GetService(typeof(SVsInfoBarUIFactory)) as IVsInfoBarUIFactory;
                IVsInfoBarUIElement element = factory.CreateInfoBar(infoBarModel);
                element.Advise(this, out _cookie);
                host.AddInfoBar(element);
            }
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

                if (notification != null && !await IsDismissedAsync(notification))
                    await Notify(notification);
            }
            catch
            {
                // ignored
            }
        }

        private class NotificationData
        {
            public string Id { get; set; }
            public string Message { get; set; }
            public string LinkText { get; set; }
            public string LinkUrl { get; set; }
        }

        private static async Task<bool> IsDismissedAsync(NotificationData notification)
        {
            //TODO: check if notification already dismissed
            return await Task.FromResult(false);
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
            await ShowInfoBar(notification.Message, notification.LinkText, notification.LinkUrl);
        }
    }
}
