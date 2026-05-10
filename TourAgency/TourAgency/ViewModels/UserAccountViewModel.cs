using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;


namespace TourAgency.ViewModels
{
    public class UserAccountViewModel : INotifyPropertyChanged
    {
        private bool _isLoggedIn = false;
        private string _userName = "Гість";
        private bool _isProfileIncomplete;
        public bool IsProfileIncomplete
        {
            get => _isProfileIncomplete;
            set { _isProfileIncomplete = value; OnPropertyChanged(); }
        }
        public ICommand OpenCompleteProfileCommand { get; }
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
  

            OpenLoginCommand = new RelayCommand(obj => OpenLogin());
            LogoutCommand = new RelayCommand(obj => Logout());
            CloseCommand = new RelayCommand(obj => CloseWindow(obj));
            RegisterCommand = new RelayCommand(obj => OpenRegister());
            OpenCompleteProfileCommand = new RelayCommand(obj => OpenCompleteProfile());
            UpdateState();
        }

        private void OpenLogin()
        {
            var loginWin = new TourAgency.View.LoginWindow();
            loginWin.ShowDialog();
            UpdateState();
        }

        public void UpdateState()
        {
            if (AuthService.CurrentUser != null)
            {
                UserName = AuthService.CurrentUser.Name;
                IsLoggedIn = true;
                IsProfileIncomplete = !AuthService.IsClientProfileFilled();
            }
            else
            {
                UserName = "Гість";
                IsLoggedIn = false;
            }
            OnPropertyChanged(nameof(IsProfileIncomplete));
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsNotLoggedIn));
        }

        private void Logout()
        {
            AuthService.Logout();
            UpdateState();
        }
        private void OpenCompleteProfile()
        {
            var win = new TourAgency.View.CompleteProfileWindow();
            win.DataContext = new CompleteProfileViewModel();
            win.ShowDialog();
            UpdateState();
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