using System;
using System.Linq;
using System.Windows;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class VacationsForDH : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        public VacationsForDH()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadData();
            LoadVacationTypes();
        }

        private void LoadData()
        {
            var vacations = _context.Vacations.ToList();
            vacationsDataGrid.ItemsSource = vacations;
        }

        private void LoadVacationTypes()
        {
            var vacationTypes = _context.Vacations.Select(v => v.Type).Distinct().ToList();
            VacationTypeComboBox.ItemsSource = vacationTypes;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string vacationType = (string)VacationTypeComboBox.SelectedItem;
            string startDateString = StartDateTextBox.Text;

            if (string.IsNullOrWhiteSpace(vacationType) && string.IsNullOrWhiteSpace(startDateString))
            {
                MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filteredVacations = _context.Vacations.ToList();

            if (!string.IsNullOrWhiteSpace(vacationType))
            {
                filteredVacations = filteredVacations.Where(v => v.Type.Contains(vacationType)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(startDateString) && DateTime.TryParse(startDateString, out DateTime startDate))
            {
                filteredVacations = filteredVacations.Where(v => v.Start_date == startDate).ToList();
            }

            vacationsDataGrid.ItemsSource = filteredVacations;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            VacationTypeComboBox.SelectedIndex = -1;
            StartDateTextBox.Text = string.Empty;
            LoadData();
        }
    }
}
