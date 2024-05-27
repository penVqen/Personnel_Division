using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ClosedXML.Excel;
using Microsoft.Win32;
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
            FillUserInfo();
            LoadData();
        }

        private void FillUserInfo()
        {
            userInfoControl.FillUserData(_user.Login, _user.Name);
        }

        private void SetImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            logoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        public void UpdateDataGrid(List<Worker> filteredWorkers)
        {
            workersDataGrid.ItemsSource = filteredWorkers;
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

            workersDataGrid.ItemsSource = workers;
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

        private void ContractsButton_Click(object sender, RoutedEventArgs e)
        {
            int? workerId = GetSelectedWorkerId();
            if (workerId.HasValue)
            {
                ContractsForDH contractsWindow = new ContractsForDH(workerId.Value);
                contractsWindow.ShowDialog();
            }
        }

        private void SanctionsButton_Click(object sender, RoutedEventArgs e)
        {
            int? workerId = GetSelectedWorkerId();
            if (workerId.HasValue)
            {
                SanctionsForDH sanctionsWindow = new SanctionsForDH(workerId.Value);
                sanctionsWindow.ShowDialog();
            }
        }

        private void VacationsButton_Click(object sender, RoutedEventArgs e)
        {
            int? workerId = GetSelectedWorkerId();
            if (workerId.HasValue)
            {
                VacationsForDH vacationsWindow = new VacationsForDH(workerId.Value);
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
