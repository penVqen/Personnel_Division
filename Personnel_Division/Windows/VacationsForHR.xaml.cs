using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class VacationsForHR : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly int _workerId;

        public VacationsForHR(int workerId)
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            _workerId = workerId;
            LoadData();
            SetTopRightImageSource();
            LoadVacationTypes();
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadData()
        {
            var vacations = _context.Worker_Vacations
                .Where(wv => wv.ID_Worker == _workerId)
                .Select(wv => new
                {
                    wv.ID_VacationNavigation.ID_Vacation,
                    wv.ID_VacationNavigation.Type,
                    wv.ID_VacationNavigation.Start_date,
                    wv.ID_VacationNavigation.End_date
                })
                .ToList();

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

            var filteredVacations = _context.Worker_Vacations
                .Where(wv => wv.ID_Worker == _workerId)
                .Select(wv => wv.ID_VacationNavigation)
                .ToList();

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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var vacationAddEditWindow = new VacationAddEdit(_context, _workerId);
            vacationAddEditWindow.ShowDialog();

            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (vacationsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите отпуск для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedVacation = (dynamic)vacationsDataGrid.SelectedItem;
            int vacationId = selectedVacation.ID_Vacation;

            var vacationAddEditWindow = new VacationAddEdit(_context, _workerId, _context.Vacations.FirstOrDefault(v => v.ID_Vacation == vacationId));
            vacationAddEditWindow.ShowDialog();

            LoadData();
        }
    }
}
