using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class VacationAddEdit : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly int _workerId;
        private readonly Vacation _vacation;

        public VacationAddEdit(OtdelKadrovPraktikaContext context, int workerId, Vacation vacation = null)
        {
            InitializeComponent();
            _context = context;
            _workerId = workerId;
            _vacation = vacation;
            SetTopRightImageSource();
            LoadVacationTypes();

            if (_vacation != null)
            {
                VacationTypeComboBox.SelectedItem = _vacation.Type;
                StartDateTextBox.Text = _vacation.Start_date.ToString("yyyy-MM-dd");
                EndDateTextBox.Text = _vacation.End_date.ToString("yyyy-MM-dd");
            }
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        private void LoadVacationTypes()
        {
            var vacationTypes = _context.Vacations
                .Select(v => v.Type)
                .Distinct()
                .ToList();

            VacationTypeComboBox.ItemsSource = vacationTypes;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (VacationTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип отпуска.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(StartDateTextBox.Text) || string.IsNullOrWhiteSpace(EndDateTextBox.Text))
            {
                MessageBox.Show("Введите даты начала и окончания отпуска.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParse(StartDateTextBox.Text, out startDate) || !DateTime.TryParse(EndDateTextBox.Text, out endDate))
            {
                MessageBox.Show("Неверный формат даты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (endDate < startDate)
            {
                MessageBox.Show("Дата окончания отпуска не может быть раньше даты начала.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var vacation = _vacation ?? new Vacation();

            vacation.Type = (string)VacationTypeComboBox.SelectedItem;
            vacation.Start_date = startDate;
            vacation.End_date = endDate;

            if (_vacation == null)
            {
                _context.Vacations.Add(vacation);

                var worker = _context.Workers.FirstOrDefault(w => w.ID_Worker == _workerId);

                _context.SaveChanges();

                var workerVacation = new Worker_Vacation
                {
                    ID_Worker = _workerId,
                    ID_Vacation = vacation.ID_Vacation
                };

                _context.Worker_Vacations.Add(workerVacation);
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
