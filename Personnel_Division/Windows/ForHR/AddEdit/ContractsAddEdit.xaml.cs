using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class ContractsAddEdit : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly int _workerId;
        private Contract _contract;

        public ContractsAddEdit(int workerId)
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            _workerId = workerId;
            SetTopRightImageSource();
            LoadContractTypes();
        }

        public ContractsAddEdit(int workerId, Contract contract) : this(workerId)
        {
            _contract = contract;
            FillData();
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadContractTypes()
        {
            var contractTypes = _context.Contracts
                .Select(c => c.Type)
                .Distinct()
                .ToList();
            ContractTypeComboBox.ItemsSource = contractTypes;
        }

        private void FillData()
        {
            if (_contract == null) return;

            ContractTypeComboBox.SelectedItem = _contract.Type;
            DateTextBox.Text = _contract.Date_conclusion;
            ValidityPeriodTextBox.Text = _contract.Validity_period;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContractTypeComboBox.Text) || string.IsNullOrWhiteSpace(DateTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ContractTypeComboBox.Text == "Срочный" && string.IsNullOrWhiteSpace(ValidityPeriodTextBox.Text))
            {
                MessageBox.Show("Вы выбрали вид договора - срочный, напишите Срок действия", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ContractTypeComboBox.Text == "Бессрочный" && !string.IsNullOrWhiteSpace(ValidityPeriodTextBox.Text))
            {
                MessageBox.Show("Вы выбрали вид договора - бессрочный, поле Срок действия должно быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dateConclusion = DateTextBox.Text;

            if (_contract == null)
            {
                var newContract = new Contract
                {
                    ID_Worker = _workerId,
                    Type = ContractTypeComboBox.Text,
                    Date_conclusion = dateConclusion,
                    Validity_period = ValidityPeriodTextBox.Text
                };

                _context.Contracts.Add(newContract);
            }
            else
            {
                var existingContract = _context.Contracts.FirstOrDefault(c => c.ID_Contract == _contract.ID_Contract);
                if (existingContract != null)
                {
                    existingContract.Type = ContractTypeComboBox.Text;
                    existingContract.Date_conclusion = dateConclusion;
                    existingContract.Validity_period = ValidityPeriodTextBox.Text;
                }
            }

            _context.SaveChanges();
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
