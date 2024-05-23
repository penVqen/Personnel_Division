using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;
using Microsoft.Win32;

namespace Personnel_Division.Windows
{
    public partial class MainForHR : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        private User _user;

        public MainForHR(User user)
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
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
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
                     w.Passport_data,
                     Date_employment = w.Date_employment.ToString("yyyy-MM-dd"),
                     w.Post,
                     w.Grade_class,
                     w.Specialization,
                     w.Working_conditions,
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
            var divisions = _context.Divisions.Select(d => d.Title).Distinct().ToList();
            divisionComboBox.ItemsSource = divisions;

            var posts = _context.Workers.Select(p => p.Post).Distinct().ToList();
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

        private int? GetSelectedWorkerId()
        {
            if (workersDataGrid.SelectedItem is null)
            {
                MessageBox.Show("Выберите сотрудника", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            dynamic selectedWorker = workersDataGrid.SelectedItem;
            return selectedWorker.ID_Worker;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            WorkersAddEdit addEditWindow = new WorkersAddEdit();
            addEditWindow.ShowDialog();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedWorker = workersDataGrid.SelectedItem;
            if (selectedWorker != null)
            {
                var workerId = ((dynamic)selectedWorker).ID_Worker;
                var worker = _context.Workers.Find(workerId);
                if (worker != null)
                {
                    WorkersAddEdit addEditWindow = new WorkersAddEdit(worker);
                    addEditWindow.ShowDialog();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserManagementButton_Click(object sender, RoutedEventArgs e)
        {
            UserManagement userManagementWindow = new UserManagement();
            userManagementWindow.ShowDialog();
        }

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            int? workerId = GetSelectedWorkerId();
            if (workerId.HasValue)
            {
                ContractsForHR contractsWindow = new ContractsForHR(workerId.Value);
                contractsWindow.ShowDialog();
            }
        }

        private void SanctionsButton_Click(object sender, RoutedEventArgs e)
        {
            int? workerId = GetSelectedWorkerId();
            if (workerId.HasValue)
            {
                SanctionsForHR sanctionsWindow = new SanctionsForHR(workerId.Value);
                sanctionsWindow.ShowDialog();
            }
        }

        private void VacationsButton_Click(object sender, RoutedEventArgs e)
        {
            int? workerId = GetSelectedWorkerId();
            if (workerId.HasValue)
            {
                VacationsForHR vacationsWindow = new VacationsForHR(workerId.Value);
                vacationsWindow.ShowDialog();
            }
        }

        private void DivisionReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var divisions = _context.Divisions
                    .Select(d => new
                    {
                        d.Title,
                        WorkerCount = d.Worker_Divisions.Count
                    })
                    .ToList();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет подразделений");

                    worksheet.Cell(1, 1).Value = "Подразделение";
                    worksheet.Cell(1, 2).Value = "Количество работников";

                    int currentRow = 2;
                    foreach (var division in divisions)
                    {
                        worksheet.Cell(currentRow, 1).Value = division.Title;
                        worksheet.Cell(currentRow, 2).Value = division.WorkerCount;
                        currentRow++;
                    }

                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        DefaultExt = ".xlsx",
                        FileName = "Отчет_подразделений.xlsx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        workbook.SaveAs(filePath);
                        MessageBox.Show($"Отчет успешно создан: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при создании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}