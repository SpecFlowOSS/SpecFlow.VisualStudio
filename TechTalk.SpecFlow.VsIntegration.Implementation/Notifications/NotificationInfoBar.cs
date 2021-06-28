using System;
using System.Collections.Generic;
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
        private readonly IBrowserNotificationService _notificationService;
        private readonly NotificationDataStore _notificationDataStore;
        private readonly NotificationData _notification;
        private uint _cookie;

        public NotificationInfoBar(IServiceProvider serviceProvider, IBrowserNotificationService notificationService, NotificationDataStore notificationDataStore, NotificationData notification)
        {
            _serviceProvider = serviceProvider;
            _notificationService = notificationService;
            _notificationDataStore = notificationDataStore;
            _notification = notification;
        }

        public void OnClosed(IVsInfoBarUIElement infoBarUIElement)
        {
            infoBarUIElement.Unadvise(_cookie);
            _notificationDataStore.SetDismissed(_notification);
        }

        public void OnActionItemClicked(IVsInfoBarUIElement infoBarUIElement, IVsInfoBarActionItem actionItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string url = (string)actionItem.ActionContext;

            _notificationService.ShowPage(url);
        }

        public Task ShowInfoBar()
        {
            return ShowInfoBar(_notification.Message, _notification.LinkText, _notification.LinkUrl);
        }

        private async Task ShowInfoBar(string message, string linkText, string linkUrl)
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
    }
}
