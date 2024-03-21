using System.Diagnostics;

namespace CiaoMondo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
        protected override void OnStart()
        {
            Debug.WriteLine("OnStart: l'app è partita");
        }
        protected override void OnSleep()
        {
            Debug.WriteLine("OnStart: l'app è in pausa");

        }
        protected override void OnResume()
        {
            Debug.WriteLine("OnStart: l'app è ripresa");
        }
    }
}
