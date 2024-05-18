using System;
using System.Linq;
using System.Windows;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class SanctionsForHR : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        public SanctionsForHR()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadData();
            LoadSanctionTypes();
        }

        private void LoadData()
        {
            var sanctions = _context.Sanctions.ToList();
            sanctionsDataGrid.ItemsSource = sanctions;
        }

        private void LoadSanctionTypes()
        {
            var sanctionTypes = _context.Sanctions.Select(s => s.Type).Distinct().ToList();
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

            var filteredSanctions = _context.Sanctions.ToList();

            if (!string.IsNullOrWhiteSpace(sanctionType))
            {
                filteredSanctions = filteredSanctions.Where(s => s.Type.Contains(sanctionType)).ToList();
            }

            if (DateTime.TryParse(dateString, out DateTime date))
            {
                filteredSanctions = filteredSanctions.Where(s => s.Date_writ == date).ToList();
            }

            sanctionsDataGrid.ItemsSource = filteredSanctions;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SanctionTypeComboBox.SelectedIndex = -1;
            DateTextBox.Text = string.Empty;
            LoadData();
        }
    }
}
