using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourAgency.Services;

namespace TourAgency.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _statusTitle;
        public bool IsAdmin => AuthService.IsAdmin;
        public ICommand OpenAccountCommand { get; }
        public ICommand AddTourCommand { get; }
        public string StatusTitle
        {
            get => _statusTitle;
            set
            {
                _statusTitle = value;
                OnPropertyChanged();
            }
        }
        

        public MainViewModel()
        {
            StatusTitle = "Вітаємо у Tour Agency!";

            OpenAccountCommand = new RelayCommand(obj => OpenAccount());
            AddTourCommand = new RelayCommand(obj => ExecuteAddTour());
        }
        private void ExecuteAddTour()
        {
            var addTourWin = new TourAgency.View.AddTourWindow();
            addTourWin.DataContext = new TourAgency.ViewModels.AddTourViewModel();
            addTourWin.ShowDialog();
        }
        private void OpenAccount()
        {
            var accountWindow = new TourAgency.View.UserAccountWindow();
            accountWindow.ShowDialog();
            OnPropertyChanged(string.Empty);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
