using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;

namespace TourAgency.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _email;
        private string _password;
        

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(obj => ExecuteLogin(obj));
            OpenRegisterCommand = new RelayCommand(obj => ExecuteOpenRegister(obj));
        }

        private void ExecuteLogin(object window)
        {
            var user = _authService.Login(Email, Password, out string message);

            if (user != null)
            {
                MessageBox.Show($"Вітаємо, {user.Name}!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                if (window is Window w) w.Close();
            }
            else
            {
                MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ExecuteOpenRegister(object window)
        {
            var currentWindow = window as Window;
            var registerWin = new TourAgency.View.RegistrationWindow();
            registerWin.Owner = Application.Current.MainWindow;
            registerWin.DataContext = new RegisterViewModel();

            registerWin.Show();
            currentWindow?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
