using System;
using System.Windows;

namespace WhisperingAudioMusicPlayer
{
    public partial class InputDialog : Window
    {
        public InputDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            lblPrompt.Content = question;
            txtResult.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtResult.SelectAll();
            txtResult.Focus();
        }

        public string Answer
        {
            get { return txtResult.Text; }
        }
    }

}