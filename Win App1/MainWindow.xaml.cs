using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace Win_App1
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = inputTextBox.Text;
            outputTextBlock.Text = "";

            // Плавне виведення символів
            foreach (char c in userInput)
            {
                outputTextBlock.Text += c;
                await Task.Delay(50); // Час затримки між символами (можеш змінити)
            }
        }
    }
}
