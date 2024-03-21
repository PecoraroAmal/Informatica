namespace Preferenze
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void PreferenzePagina(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(PreferenzePagina));
        }

        private void Local(object sender, EventArgs e)
        {

        }

        private void Leggi(object sender, EventArgs e)
        {

        }
    }

}
