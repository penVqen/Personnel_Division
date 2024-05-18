using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class MainForDH : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        private User _user;

        public MainForDH(User user)
        {
            InitializeComponent();
            _user = user;
            SetImageSource();
            _context = new OtdelKadrovPraktikaContext();
            LoadData();
            LoadComboBoxes();
            FillUserData();
        }

        private void FillUserData()
        {
            var loginTextBlock = FindName("loginTextBlock") as TextBlock;
            var nameTextBlock = FindName("nameTextBlock") as TextBlock;

            if (loginTextBlock != null)
            {
                loginTextBlock.Text = $"Логин: {_user.Login}";
            }

            if (nameTextBlock != null)
            {
                nameTextBlock.Text = $"Имя: {_user.Name}";
            }
        }

        private void SetImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\Desktop\3 курс\Курсовая 2 семестр\Personnel_Division\Personnel_Division\Images\Logo.png";
            logoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadData()
        {
            var workers = _context.Workers
                .Select(w => new
                {
                    w.ID_Worker,
                    w.Name,
                    w.Surname,
                    w.Middle_name,
                    w.Post,
                    w.Specialization,
                    w.Grade_class,
                    w.Working_conditions,
                    w.Passport_data,
                    Date_employment = w.Date_employment.ToString("yyyy-MM-dd"),
                    Date_termination = w.Date_termination.HasValue ? w.Date_termination.Value.ToString("yyyy-MM-dd") : string.Empty
                })
                .ToList();

            string selectedDivision = (string)divisionComboBox.SelectedValue;
            string selectedPost = (string)postComboBox.SelectedValue;

            if (!string.IsNullOrEmpty(selectedDivision))
            {
                var workersInSelectedDivision = _context.Worker_Divisions
                    .Where(wd => wd.ID_DivisionNavigation.Title == selectedDivision)
                    .Select(wd => wd.ID_Worker);
                workers = workers.Where(w => workersInSelectedDivision.Contains(w.ID_Worker)).ToList();
            }

            if (!string.IsNullOrEmpty(selectedPost))
            {
                workers = workers.Where(w => w.Post == selectedPost).ToList();
            }

            string name = nameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                workers = workers.Where(w => w.Name.Contains(name)).ToList();
            }

            string surname = surnameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(surname))
            {
                workers = workers.Where(w => w.Surname.Contains(surname)).ToList();
            }

            string middleName = middleNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(middleName))
            {
                workers = workers.Where(w => w.Middle_name.Contains(middleName)).ToList();
            }

            string passportData = passportTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(passportData))
            {
                workers = workers.Where(w => w.Passport_data.ToString().Contains(passportData)).ToList();
            }

            workersDataGrid.ItemsSource = workers;
        }

        private void LoadComboBoxes()
        {
            var divisions = _context.Divisions.Select(d => d.Title).ToList();
            divisionComboBox.ItemsSource = divisions;

            var posts = _context.Workers.Select(p => p.Post).ToList();
            postComboBox.ItemsSource = posts;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(nameTextBox.Text.Trim()) &&
                string.IsNullOrEmpty(surnameTextBox.Text.Trim()) &&
                string.IsNullOrEmpty(middleNameTextBox.Text.Trim()) &&
                string.IsNullOrEmpty(divisionComboBox.Text.Trim()) &&
                string.IsNullOrEmpty(postComboBox.Text.Trim()) &&
                string.IsNullOrEmpty(passportTextBox.Text.Trim()))
            {
                MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadData();
        }

        private void ResetFilters()
        {
            nameTextBox.Text = "";
            surnameTextBox.Text = "";
            middleNameTextBox.Text = "";
            divisionComboBox.SelectedIndex = -1;
            postComboBox.SelectedIndex = -1;
            passportTextBox.Text = "";
            LoadData();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFilters();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение выхода", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Authentication loginWindow = new Authentication();
                loginWindow.Show();
                Close();
            }
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            ContractsForDH contractsWindow = new ContractsForDH();
            contractsWindow.ShowDialog();
        }

        private void SanctionsButton_Click(object sender, RoutedEventArgs e)
        {
            ContractsForDH sanctionsWindow = new ContractsForDH();
            sanctionsWindow.ShowDialog();
        }

        private void VacationsButton_Click(object sender, RoutedEventArgs e)
        {
            ContractsForDH vacationsWindow = new ContractsForDH();
            vacationsWindow.ShowDialog();
        }
    }
}
