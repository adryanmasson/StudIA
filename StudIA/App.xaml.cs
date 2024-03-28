using System.Windows;

namespace StudIA
{
    public partial class App : Application
    {
        public static SessionService SessionService { get; } = new SessionService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}
