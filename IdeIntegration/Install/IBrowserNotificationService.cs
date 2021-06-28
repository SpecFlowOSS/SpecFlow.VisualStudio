using System;
using System.Windows.Forms;

namespace TechTalk.SpecFlow.IdeIntegration.Install
{
    public interface IBrowserNotificationService
    {
        bool ShowPage(string url);
    }

    class TestBrowserNotificationService : IBrowserNotificationService
    {
        public bool ShowPage(string url)
        {
            MessageBox.Show("TODO: " + url);

            return true;
        }
    }
}