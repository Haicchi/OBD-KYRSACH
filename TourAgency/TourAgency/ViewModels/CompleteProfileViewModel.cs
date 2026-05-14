using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TourAgency.Helpers;
using TourAgency.Services;


namespace TourAgency.ViewModels
{
    public class CompleteProfileViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _address;
        private string _nameTranslit;
        private string _surnameTranslit;
        private string _passportNumber;
        private string _nationality = "UKRAINIAN";
        private string _sex;
        private DateTime _birthDate = DateTime.Now.AddYears(-20);
        private DateTime _issueDate = DateTime.Now;
        private DateTime _expiryDate = DateTime.Now.AddYears(10);

        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public string NameTranslit { get => _nameTranslit; set { _nameTranslit = value; OnPropertyChanged(); } }
        public string SurnameTranslit { get => _surnameTranslit; set { _surnameTranslit = value; OnPropertyChanged(); } }
        public string PassportNumber { get => _passportNumber; set { _passportNumber = value; OnPropertyChanged(); } }
        public string Nationality { get => _nationality; set { _nationality = value; OnPropertyChanged(); } }
        public string Sex { get => _sex; set { _sex = value; OnPropertyChanged(); } }
        public DateTime BirthDate { get => _birthDate; set { _birthDate = value; OnPropertyChanged(); } }
        public DateTime IssueDate { get => _issueDate; set { _issueDate = value; OnPropertyChanged(); } }
        public DateTime ExpiryDate { get => _expiryDate; set { _expiryDate = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }

        public CompleteProfileViewModel()
        {
            SaveCommand = new RelayCommand(obj => ExecuteSave(obj));
        }

        private void ExecuteSave(object window)
        {
            string[] props = { nameof(Address), nameof(NameTranslit), nameof(SurnameTranslit),
                             nameof(PassportNumber), nameof(Nationality), nameof(Sex),
                             nameof(BirthDate), nameof(IssueDate), nameof(ExpiryDate) };

            if (props.Any(p => !string.IsNullOrEmpty(this[p])))
            {
                MessageBox.Show("Будь ласка, виправте всі помилки у формі!", "Валідація", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var newPassport = new OverseasPassport
                    {
                        NameTransliterated = NameTranslit.ToUpper(),
                        SurnameTransliterated = SurnameTranslit.ToUpper(),
                        PassportNumber = PassportNumber,
                        Nationality = Nationality.ToUpper(),
                        Sex = Sex,
                        DateOfBirth = BirthDate,
                        IssueDate = IssueDate,
                        ExpiryDate = ExpiryDate
                    };

                    db.OverseasPassports.Add(newPassport);
                    db.SaveChanges();

                    var newClient = new Client
                    {
                        Address = Address,
                        Status = "Active",
                        IDAccount = AuthService.CurrentUser.ID,
                        IDOverseasPassport = newPassport.ID
                    };

                    db.Clients.Add(newClient);
                    db.SaveChanges();
                }

                MessageBox.Show("Вітаємо! Ваш профіль мандрівника активовано.");
                if (window is Window w) w.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}");
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                string translitPattern = @"^[a-zA-Z'-]+$";

                switch (columnName)
                {
                    case nameof(Address):
                        if (string.IsNullOrWhiteSpace(Address)) error = "Вкажіть адресу проживання";
                        break;

                    case nameof(NameTranslit):
                        if (string.IsNullOrWhiteSpace(NameTranslit))
                            error = "Ім'я обов'язкове (латиниця)";
                        else if (!Regex.IsMatch(NameTranslit, translitPattern))
                            error = "Тільки латиниця, дефіс або апостроф";
                        break;

                    case nameof(SurnameTranslit):
                        if (string.IsNullOrWhiteSpace(SurnameTranslit))
                            error = "Прізвище обов'язкове (латиниця)";
                        else if (!Regex.IsMatch(SurnameTranslit, translitPattern))
                            error = "Тільки латиниця, дефіс або апостроф";
                        break;

                    case nameof(Nationality):
                        if (string.IsNullOrWhiteSpace(Nationality))
                            error = "Вкажіть вашу національність";
                        else if (!Regex.IsMatch(Nationality, translitPattern))
                            error = "Національність має бути латиницею";
                        break;

                    case nameof(PassportNumber):
                        if (string.IsNullOrWhiteSpace(PassportNumber))
                            error = "Номер паспорта обов'язковий";
                        else if (PassportNumber.Length != 8)
                            error = "Має бути рівно 8 символів";
                        break;

                    case nameof(Sex):
                        if (string.IsNullOrWhiteSpace(Sex))
                            error = "Введіть Male або Female";
                        else if (Sex != "Male" && Sex != "Female")
                            error = "Дозволено тільки Male або Female";
                        break;

                    case nameof(BirthDate):
                        if (BirthDate > DateTime.Now.AddYears(-18))
                            error = "Клієнту має бути 18+ років";
                        break;

                    case nameof(IssueDate):
                        if (IssueDate > DateTime.Now)
                            error = "Дата видачі не може бути в майбутньому";
                        else if (IssueDate < BirthDate)
                            error = "Дата видачі не може бути раніше народження";
                        break;

                    case nameof(ExpiryDate):
                        if (ExpiryDate <= IssueDate)
                            error = "Термін дії має бути більше дати видачі";
                        break;
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}