using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Personnel_Division.Models;

namespace Personnel_Division.Windows
{
    public partial class WorkersAddEdit : Window
    {
        private readonly OtdelKadrovPraktikaContext _context;
        private readonly Worker _worker;

        public WorkersAddEdit()
        {
            InitializeComponent();
            _context = new OtdelKadrovPraktikaContext();
            LoadWorkingConditions();
            LoadDivisions();
            SetTopRightImageSource();
        }

        private void SetTopRightImageSource()
        {
            string imagePath = @"C:\Users\dimanosov223\source\repos\Personnel_Division\Personnel_Division\Images\Logo.png";
            topRightLogoImage.Source = new BitmapImage(new Uri(imagePath));
        }

        public WorkersAddEdit(Worker worker) : this()
        {
            _worker = worker;
            FillWorkerData();
        }

        private void FillWorkerData()
        {
            if (_worker == null) return;

            nameTextBox.Text = _worker.Name;
            surnameTextBox.Text = _worker.Surname;
            middleNameTextBox.Text = _worker.Middle_name;
            passportDataTextBox.Text = _worker.Passport_data;
            dateOfEmploymentTextBox.Text = _worker.Date_employment.ToString("yyyy-MM-dd");
            positionTextBox.Text = _worker.Post;
            gradeTextBox.Text = _worker.Grade_class;
            specializationTextBox.Text = _worker.Specialization;
            workingConditionsComboBox.SelectedItem = _worker.Working_conditions;
            terminationDateTextBox.Text = _worker.Date_termination.HasValue ? _worker.Date_termination.Value.ToString("yyyy-MM-dd") : string.Empty;
        }

        private void LoadWorkingConditions()
        {
            var workingConditions = _context.Workers
                .Select(w => w.Working_conditions)
                .Distinct()
                .ToList();
            workingConditionsComboBox.ItemsSource = workingConditions;
        }

        private void LoadDivisions()
        {
            var divisions = _context.Divisions
                .Select(d => new { d.ID_Division, d.Title })
                .ToList();
            divisionComboBox.ItemsSource = divisions;
            divisionComboBox.DisplayMemberPath = "Title";
            divisionComboBox.SelectedValuePath = "ID_Division";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string surname = surnameTextBox.Text.Trim();
            string middleName = middleNameTextBox.Text.Trim();
            string passportData = passportDataTextBox.Text.Trim();
            string dateOfEmploymentText = dateOfEmploymentTextBox.Text.Trim();
            string position = positionTextBox.Text.Trim();
            string grade = gradeTextBox.Text.Trim();
            string specialization = specializationTextBox.Text.Trim();
            string workingConditions = workingConditionsComboBox.SelectedItem?.ToString();
            string terminationDateText = terminationDateTextBox.Text.Trim();
            int? selectedDivisionId = divisionComboBox.SelectedValue as int?;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Пожалуйста, заполните поле Имя.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(surname))
            {
                MessageBox.Show("Пожалуйста, заполните поле Фамилия.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(position))
            {
                MessageBox.Show("Пожалуйста, заполните поле Должность.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(specialization))
            {
                MessageBox.Show("Пожалуйста, заполните поле Специальность.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(workingConditions))
            {
                MessageBox.Show("Пожалуйста, заполните поле Условия работы.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(passportData))
            {
                MessageBox.Show("Пожалуйста, заполните поле Паспортные данные.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DateTime.TryParse(dateOfEmploymentText, out DateTime dateOfEmployment))
            {
                MessageBox.Show("Некорректный формат даты приема на работу.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? terminationDate = null;
            if (!string.IsNullOrEmpty(terminationDateText))
            {
                if (!DateTime.TryParse(terminationDateText, out DateTime parsedTerminationDate))
                {
                    MessageBox.Show("Некорректный формат даты увольнения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                terminationDate = parsedTerminationDate;
            }

            if (_worker == null)
            {
                Worker newWorker = new Worker
                {
                    Name = name,
                    Surname = surname,
                    Middle_name = middleName,
                    Passport_data = passportData,
                    Date_employment = dateOfEmployment,
                    Post = position,
                    Grade_class = grade,
                    Specialization = specialization,
                    Working_conditions = workingConditions,
                    Date_termination = terminationDate
                };

                _context.Workers.Add(newWorker);
                _context.SaveChanges();

                if (selectedDivisionId.HasValue)
                {
                    Worker_Division newWorkerDivision = new Worker_Division
                    {
                        ID_Worker = newWorker.ID_Worker,
                        ID_Division = selectedDivisionId.Value
                    };
                    _context.Worker_Divisions.Add(newWorkerDivision);
                }
            }
            else
            {
                _worker.Name = name;
                _worker.Surname = surname;
                _worker.Middle_name = middleName;
                _worker.Passport_data = passportData;
                _worker.Date_employment = dateOfEmployment;
                _worker.Post = position;
                _worker.Grade_class = grade;
                _worker.Specialization = specialization;
                _worker.Working_conditions = workingConditions;
                _worker.Date_termination = terminationDate;

                _context.Workers.Update(_worker);

                if (selectedDivisionId.HasValue)
                {
                    var existingWorkerDivision = _context.Worker_Divisions
                        .FirstOrDefault(wd => wd.ID_Worker == _worker.ID_Worker && wd.ID_Division == selectedDivisionId.Value);
                    if (existingWorkerDivision == null)
                    {
                        Worker_Division newWorkerDivision = new Worker_Division
                        {
                            ID_Worker = _worker.ID_Worker,
                            ID_Division = selectedDivisionId.Value
                        };
                        _context.Worker_Divisions.Add(newWorkerDivision);
                    }
                    else
                    {
                        MessageBox.Show("Сотрудник уже работает в выбранном подразделении.", "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                        divisionComboBox.SelectedItem = null;
                        return;
                    }
                }
            }

            _context.SaveChanges();
            MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
