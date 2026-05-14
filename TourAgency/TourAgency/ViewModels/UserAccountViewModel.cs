using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;
using TourAgency.View;


namespace TourAgency.ViewModels
{
    public class UserAccountViewModel : INotifyPropertyChanged
    {
        private bool _isLoggedIn = false;
        private string _userName = "Гість";
        private bool _isProfileIncomplete;
        private Client _currentClient;
        private OverseasPassport _currentPassport;
        public bool IsAdmin => AuthService.IsAdmin;
        public bool IsProfileFilled => !IsProfileIncomplete;
        public bool ShowProfileWarning => !IsAdmin && IsProfileIncomplete;

        public string FullName => AuthService.CurrentUser?.Name;
        public string UserEmail => AuthService.CurrentUser?.Email;
        public string UserPhone => AuthService.CurrentUser?.Phone;

        public string ClientAddress => _currentClient?.Address ?? "Не вказано";
        public string Nationality => _currentPassport?.Nationality ?? "—";
        public string Sex => _currentPassport?.Sex ?? "—";
        public string PassportNames => _currentPassport != null
        ? $"{_currentPassport.SurnameTransliterated} {_currentPassport.NameTransliterated}"
        : "Дані відсутні";

        public string PassportNumber => _currentPassport?.PassportNumber ?? "Дані відсутні";

        public string ExpiryDateInfo => _currentPassport != null
        ? _currentPassport.ExpiryDate.ToString("dd.MM.yyyy")
        : "—";

        
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

        public ICommand OpenFullInfoCommand { get; }
        public ICommand OpenMyToursCommand { get; }
        public ICommand OpenAdminPanelCommand { get; }

        public UserAccountViewModel()
        {

            LoadClientData();
            OpenLoginCommand = new RelayCommand(obj => OpenLogin());
            LogoutCommand = new RelayCommand(obj => Logout());
            CloseCommand = new RelayCommand(obj => CloseWindow(obj));
            RegisterCommand = new RelayCommand(obj => OpenRegister());
            OpenCompleteProfileCommand = new RelayCommand(obj => OpenCompleteProfile());
            OpenFullInfoCommand = new RelayCommand(obj => ExecuteOpenFullInfo());
            OpenMyToursCommand = new RelayCommand(obj => ExecuteOpenMyTours());
   
            UpdateState();
        }
        public void LoadClientData()
        {
            var currentUserId = AuthService.CurrentUser?.ID;
            if (currentUserId == null) return;

            using (var db = new AppDbContext())
            {
                _currentClient = db.Clients
                    .FirstOrDefault(c => c.IDAccount == currentUserId);

                if (_currentClient != null)
                {
                    _currentPassport = db.OverseasPassports
                        .FirstOrDefault(p => p.ID == _currentClient.IDOverseasPassport);
                }
            }
        }
        private void ExecuteOpenFullInfo()
        {
            var infoWin = new FullInfoWindow();
            infoWin.DataContext = this;

            infoWin.ShowDialog();
        }

        private void ExecuteOpenMyTours()
        {
            MessageBox.Show("Список ваших турів порожній. Час щось забронювати!", "Мої тури");
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
                LoadClientData();

                IsProfileIncomplete = !AuthService.IsClientProfileFilled();
            }
            else
            {
                UserName = "Гість";
                IsLoggedIn = false;
                _currentClient = null;
                _currentPassport = null;
            }
            OnPropertyChanged(nameof(IsProfileIncomplete));
            OnPropertyChanged(nameof(IsProfileFilled));
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsNotLoggedIn));
            OnPropertyChanged(nameof(ShowProfileWarning));
            OnPropertyChanged(string.Empty); 
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
            UpdateState();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}