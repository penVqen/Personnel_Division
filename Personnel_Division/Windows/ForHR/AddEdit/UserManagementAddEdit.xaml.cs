using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class UserManagementAddEdit : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly User _user;

        public UserManagementAddEdit()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            SetTopRightImageSource();
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        public UserManagementAddEdit(User user) : this()
        {
            _user = user;
            LoginTextBox.Text = user.Login;
            NameTextBox.Text = user.Name;
            PasswordTextBox.Text = user.Password;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string name = NameTextBox.Text;
            string password = PasswordTextBox.Text;

            if (!login.StartsWith("HR") && !login.StartsWith("DH"))
            {
                MessageBox.Show("Логин должен начинаться или с HR (сотрудник отдела кадров), или DH (начальник подразделения).", "Ошибка ввода логина", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Поле 'Имя' не должно быть пустым.", "Ошибка ввода имени", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string errorMessage = GetPasswordErrorMessage(password);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Ошибка ввода пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_user == null)
            {
                var existingUser = _context.Users.FirstOrDefault(u => u.Login == login);
                if (existingUser != null)
                {
                    MessageBox.Show($"Пользователь с логином '{login}' уже существует.", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                User newUser = new User
                {
                    Login = login,
                    Name = name,
                    Password = password
                };

                _context.Users.Add(newUser);
            }
            else
            {
                _user.Login = login;
                _user.Name = name;
                _user.Password = password;
                _context.Users.Update(_user);
            }

            _context.SaveChanges();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string GetPasswordErrorMessage(string password)
        {
            if (password.Length < 6)
                return "Пароль должен содержать минимум 6 символов.";

            if (!password.Any(char.IsUpper))
                return "Пароль должен содержать минимум 1 прописную букву.";

            if (!password.Any(char.IsDigit))
                return "Пароль должен содержать минимум 1 цифру.";

            string specialCharacters = "!@#$%^";
            if (!password.Any(specialCharacters.Contains))
                return "Пароль должен содержать минимум один из следующих символов: ! @ # $ % ^.";

            return null;
        }
    }
}
