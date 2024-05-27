using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class SanctionsForHR : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly int _workerId;

        public SanctionsForHR(int workerId)
        {
            InitializeComponent();
            _workerId = workerId;
            _context = new OtdelKadrovPraktikaContext();
            LoadData();
            SetTopRightImageSource();
            LoadSanctionTypes();
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadData()
        {
            var sanctions = _context.Sanctions
                .Where(s => s.ID_Worker == _workerId)
                .ToList();

            sanctionsDataGrid.ItemsSource = sanctions;
        }

        private void LoadSanctionTypes()
        {
            var sanctionTypes = _context.Sanctions
                .Select(s => s.Type)
                .Distinct()
                .ToList();

            SanctionTypeComboBox.ItemsSource = sanctionTypes;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string sanctionType = (string)SanctionTypeComboBox.SelectedItem;
            string dateString = DateTextBox.Text;

            if (string.IsNullOrWhiteSpace(sanctionType) && string.IsNullOrWhiteSpace(dateString))
            {
                MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filteredSanctions = _context.Sanctions
                .Where(s => s.ID_Worker == _workerId)
                .ToList();

            if (!string.IsNullOrWhiteSpace(sanctionType))
            {
                filteredSanctions = filteredSanctions
                    .Where(s => s.Type.Contains(sanctionType))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(dateString) && DateTime.TryParse(dateString, out DateTime date))
            {
                filteredSanctions = filteredSanctions
                    .Where(s => s.Date_writ == date)
                    .ToList();
            }

            sanctionsDataGrid.ItemsSource = filteredSanctions;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SanctionTypeComboBox.SelectedIndex = -1;
            DateTextBox.Text = string.Empty;
            LoadData();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new SanctionAddEdit(_workerId);
            addEditWindow.ShowDialog();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sanctionsDataGrid.SelectedItem is Sanction selectedSanction)
            {
                var addEditWindow = new SanctionAddEdit(_workerId, selectedSanction);
                addEditWindow.ShowDialog();
                LoadData();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите санкцию для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
