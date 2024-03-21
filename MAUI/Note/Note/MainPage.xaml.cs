using Microsoft.Maui;

namespace Note
{
    public partial class MainPage : ContentPage
    {
        string _fileName = Path.Combine(FileSystem.AppDataDirectory, "notes.txt");

        public MainPage()
        {
            InitializeComponent();

            if (File.Exists(_fileName))
            {
                editor.Text = File.ReadAllText(_fileName);
            }
        }
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if(editor.Text != null)
            {
                File.WriteAllText(_fileName, editor.Text);
                DisplayAlert("Stato", "Operazione eseguita con successo", "ok");
                editor.Text = File.ReadAllText(_fileName);
            }
        }

        private void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            editor.Text = null;
        }
    }

}