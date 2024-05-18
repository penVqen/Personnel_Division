using System;
using System.Linq;
using System.Windows;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class UserManagement : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        public UserManagement()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadData();
        }

        private void LoadData()
        {
            var users = _context.Users.ToList();
            usersDataGrid.ItemsSource = users;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string name = NameTextBox.Text;

            if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filteredUsers = _context.Users.ToList();

            if (!string.IsNullOrWhiteSpace(login))
            {
                filteredUsers = filteredUsers.Where(u => u.Login.Contains(login)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(name)).ToList();
            }

            usersDataGrid.ItemsSource = filteredUsers;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = string.Empty;
            NameTextBox.Text = string.Empty;
            LoadData();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagementAddEdit addEditWindow = new UserManagementAddEdit();
            addEditWindow.Closed += (s, args) => LoadData();
            addEditWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (usersDataGrid.SelectedItem is User selectedUser)
            {
                UserManagementAddEdit addEditWindow = new UserManagementAddEdit(selectedUser);
                addEditWindow.Closed += (s, args) => LoadData();
                addEditWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (usersDataGrid.SelectedItem is User selectedUser)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Users.Remove(selectedUser);
                    _context.SaveChanges();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
