using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace TourAgency.ViewModels
{
    public class UserAccountViewModel : INotifyPropertyChanged
    {
        private bool _isLoggedIn = false;
        private string _userName = "Гість";

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoggedIn));
            }
        }

        public bool IsNotLoggedIn => !IsLoggedIn;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenLoginCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand CloseCommand { get; }

        public ICommand RegisterCommand {  get; }

        public UserAccountViewModel()
        {
            IsLoggedIn = false;

            OpenLoginCommand = new RelayCommand(obj => OpenLogin());
            LogoutCommand = new RelayCommand(obj => Logout());
            CloseCommand = new RelayCommand(obj => CloseWindow(obj));
            RegisterCommand = new RelayCommand(obj => OpenRegister());
        }

        private void OpenLogin()
        {
            MessageBox.Show("Відкриваємо вікно входу...");
            IsLoggedIn = true;
            UserName = "Кирило";
        }

        private void Logout()
        {
            IsLoggedIn = false;
            UserName = "Гість";
        }

        private void CloseWindow(object window)
        {
            if (window is Window w)
            {
                w.Close();
            }
        }
        private void OpenRegister()
        {
            var registerWindow = new TourAgency.View.RegistrationWindow();
          
            registerWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}