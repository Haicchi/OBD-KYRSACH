using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;


namespace TourAgency.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
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
            if (window is Window w)
            {
                w.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}