using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;

namespace TourAgency.ViewModels
{
    public class AdminToolsViewModel : INotifyPropertyChanged
    {
        private string _nameInput;
        private Country _selectedCountry;
        private Town _selectedTown;
        private string _hotelName;
        private string _hotelDescription;
        private string _hotelAddress;
        private int _hotelRating = 3;

        private ObservableCollection<Country> _allCountries;
        private ObservableCollection<Town> _allTowns;

        public ObservableCollection<Country> AllCountries
        {
            get => _allCountries;
            set { _allCountries = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Town> AllTowns
        {
            get => _allTowns;
            set { _allTowns = value; OnPropertyChanged(); }
        }

        public string NameInput { get => _nameInput; set { _nameInput = value; OnPropertyChanged(); } }
        public Country SelectedCountry { get => _selectedCountry; set { _selectedCountry = value; OnPropertyChanged(); } }
        public Town SelectedTown { get => _selectedTown; set { _selectedTown = value; OnPropertyChanged(); } }

        public string HotelName { get => _hotelName; set { _hotelName = value; OnPropertyChanged(); } }
        public string HotelDescription { get => _hotelDescription; set { _hotelDescription = value; OnPropertyChanged(); } }
        public string HotelAddress { get => _hotelAddress; set { _hotelAddress = value; OnPropertyChanged(); } }
        public int HotelRating { get => _hotelRating; set { _hotelRating = value; OnPropertyChanged(); } }

        public ICommand ConfirmCommand { get; }
        public ICommand CloseCommand { get; }

        public AdminToolsViewModel()
        {
            LoadBaseData();
            ConfirmCommand = new RelayCommand(ExecuteConfirm);
            CloseCommand = new RelayCommand(obj => (obj as Window)?.Close());
        }

        private void LoadBaseData()
        {
            using (var db = new AppDbContext())
            {
                AllCountries = new ObservableCollection<Country>(db.Countries.ToList());
                AllTowns = new ObservableCollection<Town>(db.Towns.ToList());
            }
        }

        private void ExecuteConfirm(object parameter)
        {
            var p = parameter as string;
            var window = Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);

            try
            {
                using (var db = new AppDbContext())
                {
                    switch (p)
                    {
                        case "Country":
                            if (string.IsNullOrWhiteSpace(NameInput)) return;
                            // Перевірка на дублікат
                            if (db.Countries.Any(c => c.Name.ToLower() == NameInput.ToLower()))
                            {
                                MessageBox.Show("Така країна вже існує!");
                                return;
                            }
                            db.Countries.Add(new Country { Name = NameInput });
                            break;

                        case "Town":
                            if (string.IsNullOrWhiteSpace(NameInput) || SelectedCountry == null) return;
                            // Перевірка: чи є в цій країні вже таке місто
                            if (db.Towns.Any(t => t.Name.ToLower() == NameInput.ToLower() && t.IDCountry == SelectedCountry.ID))
                            {
                                MessageBox.Show("Це місто вже додане до обраної країни!");
                                return;
                            }
                            db.Towns.Add(new Town { Name = NameInput, IDCountry = SelectedCountry.ID });
                            break;

                        case "Hotel":
                            if (string.IsNullOrWhiteSpace(HotelName) || SelectedTown == null) return;
                            // Перевірка за назвою та адресою готелю
                            if (db.Hotels.Any(h => h.Name.ToLower() == HotelName.ToLower() && h.Address.ToLower() == HotelAddress.ToLower()))
                            {
                                MessageBox.Show("Готель з такою назвою за цією адресою вже зареєстрований!");
                                return;
                            }
                            db.Hotels.Add(new Hotel
                            {
                                Name = HotelName,
                                Description = HotelDescription,
                                Address = HotelAddress,
                                Rating = HotelRating,
                                IDTown = SelectedTown.ID
                            });
                            break;

                        case "Food":
                            if (string.IsNullOrWhiteSpace(NameInput)) return;
                            if (db.Foods.Any(f => f.Type.ToLower() == NameInput.ToLower()))
                            {
                                MessageBox.Show("Цей тип харчування вже є в базі!");
                                return;
                            }
                            db.Foods.Add(new Food { Type = NameInput });
                            break;

                        case "Type":
                            if (string.IsNullOrWhiteSpace(NameInput)) return;
                            if (db.TourTypes.Any(t => t.Type.ToLower() == NameInput.ToLower()))
                            {
                                MessageBox.Show("Цей тип відпочинку вже існує!");
                                return;
                            }
                            db.TourTypes.Add(new TourType { Type = NameInput });
                            break;
                    }
                    db.SaveChanges();
                }

                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
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