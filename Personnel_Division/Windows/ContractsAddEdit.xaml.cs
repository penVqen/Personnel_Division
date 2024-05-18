using System;
using System.Linq;
using System.Windows;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    /// <summary>
    /// Логика взаимодействия для ContractsAddEdit.xaml
    /// </summary>
    public partial class ContractsAddEdit : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;

        public ContractsAddEdit()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadContractTypes();
        }

        private void LoadContractTypes()
        {
            var contractTypes = _context.Contracts.Select(c => c.Type).Distinct().ToList();
            ContractTypeComboBox.ItemsSource = contractTypes;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Реализуйте логику сохранения данных здесь
            // Пример:
            // var contractType = ContractTypeComboBox.SelectedItem?.ToString();
            // var dateConclusion = DateTextBox.Text;
            // var validityPeriod = ValidityPeriodTextBox.Text;
            // Добавьте логику сохранения в базу данных и обновления данных
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
