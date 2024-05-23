using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class SanctionAddEdit : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly int _workerId;
        private readonly Sanction _sanction;

        public SanctionAddEdit(int workerId)
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            _workerId = workerId;
            SetTopRightImageSource();
            LoadSanctionTypes();
        }

        public SanctionAddEdit(int workerId, Sanction sanction) : this(workerId)
        {
            _sanction = sanction;
            FillData();
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadSanctionTypes()
        {
            var sanctionTypes = _context.Sanctions
                .Select(s => s.Type)
                .Distinct()
                .ToList();

            ContractTypeComboBox.ItemsSource = sanctionTypes;
        }

        private void FillData()
        {
            if (_sanction == null) return;

            ContractTypeComboBox.SelectedItem = _sanction.Type;
            WritTextBox.Text = _sanction.Writ;
            DateTextBox.Text = _sanction.Date_writ.ToString("yyyy-MM-dd");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContractTypeComboBox.Text) || string.IsNullOrWhiteSpace(DateTextBox.Text) || string.IsNullOrWhiteSpace(WritTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DateTime.TryParse(DateTextBox.Text, out DateTime date))
            {
                MessageBox.Show("Пожалуйста, введите корректную дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_sanction == null)
            {
                var newSanction = new Sanction
                {
                    ID_Worker = _workerId,
                    Type = ContractTypeComboBox.Text,
                    Date_writ = date,
                    Writ = WritTextBox.Text
                };

                _context.Sanctions.Add(newSanction);
            }
            else
            {
                _sanction.Type = ContractTypeComboBox.Text;
                _sanction.Date_writ = date;
                _sanction.Writ = WritTextBox.Text;
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
