using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;
using TourAgency.Helpers;

namespace TourAgency.ViewModels
{
    public class CompleteProfileViewModel : INotifyPropertyChanged
    {
        
        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public string NameTranslit { get; set; }
        public string SurnameTranslit { get; set; }
        public string PassportNumber { get; set; }
        public string Nationality { get; set; } = "UKRAINIAN";
        public string Sex { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.Now.AddYears(-20);
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(10);

        public ICommand SaveCommand { get; }

        public CompleteProfileViewModel()
        {
            SaveCommand = new RelayCommand(obj => ExecuteSave(obj));
        }

        private void ExecuteSave(object window)
        {
            if (string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(PassportNumber) || string.IsNullOrWhiteSpace(NameTranslit))
            {
                MessageBox.Show("Будь ласка, заповніть основні поля (Адреса, Прізвище/Ім'я, Номер паспорта)!");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                { 
                    var newPassport = new OverseasPassport
                    {
                        NameTransliterated = NameTranslit,
                        SurnameTransliterated = SurnameTranslit,
                        PassportNumber = PassportNumber,
                        Nationality = Nationality,
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
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}