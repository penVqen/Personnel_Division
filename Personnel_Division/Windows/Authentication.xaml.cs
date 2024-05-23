using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;
using Personnel_Division.Windows;

namespace Personnel_Division
{
    /// <summary>
    /// Interaction logic for Authentication.xaml
    /// </summary>
    public partial class Authentication : Window
    {
        private readonly OtdelKadrovPraktikaContext _dbContext;

        public Authentication()
        {
            InitializeComponent();
            _dbContext = new OtdelKadrovPraktikaContext();
            SetImageSource();
        }

        private void SetImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            logoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = passBoxPassword.Password;

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                if (login.StartsWith("HR"))
                {
                    MainForHR mainForHR = new MainForHR(user);
                    mainForHR.Show();
                }
                else if (login.StartsWith("DH"))
                {
                    MainForDH mainForDH = new MainForDH(user);
                    mainForDH.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
