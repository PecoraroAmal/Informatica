namespace Preferenze
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(PreferencePage), typeof(PreferencePage));
        }
    }
}
