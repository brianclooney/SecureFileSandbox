using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureFile.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {
                DragMove();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginRequest = new
            {
                UserName = UsernameBox.Text,
                Password = PasswordBox.Password
            };

            var json = JsonConvert.SerializeObject(loginRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync("http://192.168.27.89:8384/users/login", data);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var msg = await response.Content.ReadAsStringAsync();
                MessageBox.Show(msg);
            }
            else
            {
                MessageBox.Show("Login Failed!");
            }
        }
    }
}