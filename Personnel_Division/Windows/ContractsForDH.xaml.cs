using System;
using System.Linq;
using System.Windows;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class ContractsForDH : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        public ContractsForDH()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadData();
            LoadContractTypes();
        }

        private void LoadData()
        {
            var contracts = _context.Contracts.ToList();
            contractsDataGrid.ItemsSource = contracts;
        }

        private void LoadContractTypes()
        {
            var contractTypes = _context.Contracts.Select(c => c.Type).Distinct().ToList();
            ContractTypeComboBox.ItemsSource = contractTypes;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string contractType = (string)ContractTypeComboBox.SelectedItem;
            string dateString = DateTextBox.Text;

            if (string.IsNullOrWhiteSpace(contractType) && string.IsNullOrWhiteSpace(dateString))
            {
                MessageBox.Show("Введите данные для фильтрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var filteredContracts = _context.Contracts.ToList();

            if (!string.IsNullOrWhiteSpace(contractType))
            {
                filteredContracts = filteredContracts.Where(c => c.Type.Contains(contractType)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(dateString))
            {
                filteredContracts = filteredContracts.Where(c => c.Date_conclusion == dateString).ToList();
            }

            contractsDataGrid.ItemsSource = filteredContracts;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ContractTypeComboBox.SelectedIndex = -1;
            DateTextBox.Text = string.Empty;
            LoadData();
        }
    }
}
