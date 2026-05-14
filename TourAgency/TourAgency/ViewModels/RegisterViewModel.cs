using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;
using TourAgency.View;


namespace TourAgency.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly AuthService _authService;

        private string _name;
        private string _email;
        private string _phone;
        private string _password;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand OpenLoginCommand { get; }

        public RegisterViewModel()
        {
            _authService = new AuthService();
            RegisterCommand = new RelayCommand(obj => ExecuteRegister(obj));
            OpenLoginCommand = new RelayCommand(obj => ExecuteOpenLogin(obj));
        }

        private void ExecuteRegister(object window)
        {
            bool isSuccess = _authService.Register(Name, Email, Phone, Password, out string message);

            if (isSuccess)
            {
                MessageBox.Show(message, "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window is Window w)
                {
                    w.Close();
                }
            }
            else
            {
                MessageBox.Show(message, "Помилка реєстрації", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ExecuteOpenLogin(object window)
        {

            var loginWin = new LoginWindow();

            if (window is Window currentWindow)
            {
                currentWindow.Close(); 
            }

            loginWin.ShowDialog();
        }
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                switch (columnName)
                {
                    case nameof(Name):
                        if (string.IsNullOrWhiteSpace(Name))
                            error = "ПІБ обов'язкове";
                        else
                        {
                            var parts = Name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length != 3)
                                error = "Введіть Прізвище, Ім'я та По батькові";
                        }
                        break;

                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                            error = "Email обов'язковий";
                        else if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                            error = "Невірний формат Email";
                        break;

                    case nameof(Phone):
                        if (string.IsNullOrWhiteSpace(Phone))
                            error = "Номер телефону обов'язковий";
                        else if (Phone.Length != 13)
                            error = "Має бути рівно 13 символів (+380...)";
                        break;

                    case nameof(Password):
                        if (string.IsNullOrWhiteSpace(Password))
                            error = "Пароль обов'язковий";
                        else if (Password.Length < 6)
                            error = "Мінімум 6 символів";
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