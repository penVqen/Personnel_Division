using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class ContractsForDH : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly int _workerId;

        public ContractsForDH(int workerId)
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            _workerId = workerId;
            SetImageSource();
            LoadContractTypes();
            LoadData();
        }

        private void SetImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadData()
        {
            var contracts = _context.Contracts
                .Where(c => c.ID_Worker == _workerId)
                .Select(c => new
                {
                    c.ID_Contract,
                    c.Type,
                    c.Validity_period,
                    c.Date_conclusion
                })
                .ToList();

            contractsDataGrid.ItemsSource = contracts;
        }

        private void LoadContractTypes()
        {
            var contractTypes = _context.Contracts
                .Select(c => c.Type)
                .Distinct()
                .ToList();
            ContractTypeComboBox.ItemsSource = contractTypes;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedContractType = (string)ContractTypeComboBox.SelectedItem;
            string dateString = DateTextBox.Text.Trim();

            if (string.IsNullOrEmpty(selectedContractType) && string.IsNullOrEmpty(dateString))
            {
                MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var contracts = _context.Contracts
                .Where(c => c.ID_Worker == _workerId)
                .ToList();

            if (!string.IsNullOrEmpty(selectedContractType))
            {
                contracts = contracts
                    .Where(c => c.Type == selectedContractType)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(dateString))
            {
                contracts = contracts
                    .Where(c => c.Date_conclusion == dateString)
                    .ToList();
            }

            contractsDataGrid.ItemsSource = contracts;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ContractTypeComboBox.SelectedIndex = -1;
            DateTextBox.Text = string.Empty;
            LoadData();
        }
    }
}
