using System.Diagnostics;

namespace Login
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent(); 
            //MyStackLayout.Padding =
            //    (DeviceInfo.Platform == DevicePlatform.iOS)?
            //    new Thickness(30, 60, 30, 30) // Shift down by 60 points on iOS only
            //    : new Thickness(30); // Set the default margin to be 30 points
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;
            DisplayAlert("Login", $"Login Clicked with Username: {username} and password {password}", "Ok");
            Debug.WriteLine("Clicked!");
        }

        private void Password_Tapped(object sender, TappedEventArgs e)
        {
            if(Password.IsPassword)
            {
                Password.IsPassword = false;
            }
            else
            {
                Password.IsPassword = true;
            }
        }
    }
}
