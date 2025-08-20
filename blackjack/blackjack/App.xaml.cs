using System.Windows;
using BlackjackApp.helpers;

namespace blackjack
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DatabaseHelper.EnsureChipsColumn();
        }
    }
}