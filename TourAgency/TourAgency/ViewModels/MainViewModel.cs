using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace TourAgency.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _statusTitle;
        public ICommand OpenAccountCommand { get; }
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
        }

        private void OpenAccount()
        {
            var accountWindow = new TourAgency.View.UserAccountWindow();
            accountWindow.ShowDialog();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
