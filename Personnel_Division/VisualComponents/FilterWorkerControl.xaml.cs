using System.Windows;
using System.Windows.Controls;
using Personnel_Division.Models;
using System.Collections.Generic;
using System.Linq;
using Personnel_Division.Windows;

namespace Personnel_Division.VisualComponents
{
    public partial class FilterWorkerControl : UserControl
    {
        private readonly OtdelKadrovPraktikaContext _context;

        public FilterWorkerControl()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            var divisions = _context.Divisions.Select(d => d.Title).Distinct().ToList();
            DivisionComboBox.ItemsSource = divisions;

            var posts = _context.Workers.Select(p => p.Post).Distinct().ToList();
            PostComboBox.ItemsSource = posts;
        }

        public List<Worker> ApplyFilters(List<Worker> workers)
        {
            string selectedDivision = (string)DivisionComboBox.SelectedValue;
            string selectedPost = (string)PostComboBox.SelectedValue;

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

            string name = NameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(name))
            {
                workers = workers.Where(w => w.Name.Contains(name)).ToList();
            }

            string surname = SurnameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(surname))
            {
                workers = workers.Where(w => w.Surname.Contains(surname)).ToList();
            }

            string middleName = MiddleNameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(middleName))
            {
                workers = workers.Where(w => w.Middle_name.Contains(middleName)).ToList();
            }

            string passportData = PassportTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(passportData))
            {
                workers = workers.Where(w => w.Passport_data.StartsWith(passportData)).ToList();
            }

            return workers;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = Window.GetWindow(this);

            if (mainWindow is MainForHR mainWindowForHR || mainWindow is MainForDH mainWindowForDH)
            {
                if (string.IsNullOrEmpty((string)DivisionComboBox.SelectedValue) &&
                    string.IsNullOrEmpty((string)PostComboBox.SelectedValue) &&
                    string.IsNullOrEmpty(NameTextBox.Text.Trim()) &&
                    string.IsNullOrEmpty(SurnameTextBox.Text.Trim()) &&
                    string.IsNullOrEmpty(MiddleNameTextBox.Text.Trim()) &&
                    string.IsNullOrEmpty(PassportTextBox.Text.Trim()))
                {
                    MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<Worker> workers = _context.Workers.ToList();
                List<Worker> filteredWorkers = ApplyFilters(workers);

                if (mainWindow is MainForHR)
                {
                    ((MainForHR)mainWindow).UpdateDataGrid(filteredWorkers);
                }
                else if (mainWindow is MainForDH)
                {
                    ((MainForDH)mainWindow).UpdateDataGrid(filteredWorkers);
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DivisionComboBox.SelectedIndex = -1;
            PostComboBox.SelectedIndex = -1;
            NameTextBox.Text = "";
            SurnameTextBox.Text = "";
            MiddleNameTextBox.Text = "";
            PassportTextBox.Text = "";

            Window mainWindow = Window.GetWindow(this);

            if (mainWindow is MainForHR mainWindowForHR)
            {
                List<Worker> workers = _context.Workers.ToList();
                List<Worker> filteredWorkers = ApplyFilters(workers);
                mainWindowForHR.UpdateDataGrid(filteredWorkers);
            }
            else if (mainWindow is MainForDH mainWindowForDH)
            {
                List<Worker> workers = _context.Workers.ToList();
                List<Worker> filteredWorkers = ApplyFilters(workers);
                mainWindowForDH.UpdateDataGrid(filteredWorkers);
            }
        }
    }
}
