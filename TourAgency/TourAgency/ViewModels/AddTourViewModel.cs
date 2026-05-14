using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourAgency.Services;
using TourAgency.View;


namespace TourAgency.ViewModels
{
    public class AddTourViewModel : INotifyPropertyChanged
    {
        private string _tourName;
        private string _description;
        private string _photoPath;
        private DateTime _departureDate = DateTime.Now.AddDays(7);
        private DateTime _arrivalDate = DateTime.Now.AddDays(14);
        private int _totalSeats;

        private Country _selectedCountry;
        private Town _selectedTown;
        private Hotel _selectedHotel;
        private Food _selectedFood;
        private TourType _selectedTourType;

        public ObservableCollection<Country> Countries { get; set; }
        public ObservableCollection<Town> FilteredTowns { get; set; }
        public ObservableCollection<Hotel> FilteredHotels { get; set; }
        public ObservableCollection<Food> Foods { get; set; }
        public ObservableCollection<TourType> TourTypes { get; set; }

        public string TourName { get => _tourName; set { _tourName = value; OnPropertyChanged(); } }
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }
        public string PhotoPath { get => _photoPath; set { _photoPath = value; OnPropertyChanged(); } }
        public DateTime DepartureDate { get => _departureDate; set { _departureDate = value; OnPropertyChanged(); } }
        public DateTime ArrivalDate { get => _arrivalDate; set { _arrivalDate = value; OnPropertyChanged(); } }
        public int TotalSeats
        {
            get => _totalSeats;
            set
            {
                if (value >= 0) 
                {
                    _totalSeats = value;
                    OnPropertyChanged();
                }
            }
        }

        public Country SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                OnPropertyChanged();
                LoadFilteredTowns();
            }
        }

        public Town SelectedTown
        {
            get => _selectedTown;
            set
            {
                _selectedTown = value;
                OnPropertyChanged();
                LoadFilteredHotels();
            }
        }

        public Hotel SelectedHotel { get => _selectedHotel; set { _selectedHotel = value; OnPropertyChanged(); } }
        public Food SelectedFood { get => _selectedFood; set { _selectedFood = value; OnPropertyChanged(); } }
        public TourType SelectedTourType { get => _selectedTourType; set { _selectedTourType = value; OnPropertyChanged(); } }

        public ICommand SelectPhotoCommand { get; }
        public ICommand SaveTourCommand { get; }
        public ICommand CloseCommand { get; }

        public ICommand OpenAddCountryCommand { get; }
        public ICommand OpenAddTownCommand { get; }
        public ICommand OpenAddHotelCommand { get; }
        public ICommand OpenAddFoodCommand { get; }
        public ICommand OpenAddTourTypeCommand { get; }

        public AddTourViewModel()
        {
            LoadDictionaries();

            SelectPhotoCommand = new RelayCommand(obj => SelectPhoto());
            SaveTourCommand = new RelayCommand(obj => SaveTour());
            CloseCommand = new RelayCommand(obj => (obj as Window)?.Close());

            OpenAddCountryCommand = new RelayCommand(obj => ExecuteOpenAddCountry());
            OpenAddTownCommand = new RelayCommand(obj => ExecuteOpenAddTown());
            OpenAddHotelCommand = new RelayCommand(obj => ExecuteOpenAddHotel());
            OpenAddFoodCommand = new RelayCommand(obj => ExecuteOpenAddFood());
            OpenAddTourTypeCommand = new RelayCommand(obj => ExecuteOpenAddTourType());
        }

        private void LoadDictionaries()
        {
            using (var db = new AppDbContext())
            {
                Countries = new ObservableCollection<Country>(db.Countries.ToList());
                Foods = new ObservableCollection<Food>(db.Foods.ToList());
                TourTypes = new ObservableCollection<TourType>(db.TourTypes.ToList());
                OnPropertyChanged(nameof(Countries));
                OnPropertyChanged(nameof(Foods));
                OnPropertyChanged(nameof(TourTypes));
            }
        }

        private void LoadFilteredTowns()
        {
            if (SelectedCountry == null) return;
            using (var db = new AppDbContext())
            {
                FilteredTowns = new ObservableCollection<Town>(
                    db.Towns.Where(t => t.IDCountry == SelectedCountry.ID).ToList());
                OnPropertyChanged(nameof(FilteredTowns));
            }
        }

        private void LoadFilteredHotels()
        {
            if (SelectedTown == null) return;
            using (var db = new AppDbContext())
            {
                FilteredHotels = new ObservableCollection<Hotel>(
                    db.Hotels.Where(h => h.IDTown == SelectedTown.ID).ToList());
                OnPropertyChanged(nameof(FilteredHotels));
            }
        }

        private void ExecuteOpenAddCountry()
        {
            var adminVM = new AdminToolsViewModel();
            var win = new AddCountryWindow { DataContext = adminVM, Owner = Application.Current.Windows.OfType<AddTourWindow>().FirstOrDefault() };
            if (win.ShowDialog() == true) LoadDictionaries();
        }

        private void ExecuteOpenAddTown()
        {
            var adminVM = new AdminToolsViewModel();
            var win = new AddTownWindow { DataContext = adminVM, Owner = Application.Current.Windows.OfType<AddTourWindow>().FirstOrDefault() };
            if (win.ShowDialog() == true) LoadFilteredTowns();
        }

        private void ExecuteOpenAddHotel()
        {
            var adminVM = new AdminToolsViewModel();
            var win = new AddHotelWindow { DataContext = adminVM, Owner = Application.Current.Windows.OfType<AddTourWindow>().FirstOrDefault() };
            if (win.ShowDialog() == true) LoadFilteredHotels();
        }

        private void ExecuteOpenAddFood()
        {
            var adminVM = new AdminToolsViewModel();
            var win = new AddFoodWindow { DataContext = adminVM, Owner = Application.Current.Windows.OfType<AddTourWindow>().FirstOrDefault() };
            if (win.ShowDialog() == true) LoadDictionaries();
        }

        private void ExecuteOpenAddTourType()
        {
            var adminVM = new AdminToolsViewModel();
            var win = new AddTourTypeWindow { DataContext = adminVM, Owner = Application.Current.Windows.OfType<AddTourWindow>().FirstOrDefault() };
            if (win.ShowDialog() == true) LoadDictionaries();
        }

        private void SelectPhoto()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Images (*.jpg;*.png)|*.jpg;*.png";
            if (dlg.ShowDialog() == true) PhotoPath = dlg.FileName;
        }

        private void SaveTour()
        {
            if (string.IsNullOrEmpty(TourName) || SelectedHotel == null || SelectedFood == null || string.IsNullOrEmpty(PhotoPath))
            {
                MessageBox.Show("Будь ласка, заповніть основні поля та оберіть фото!");
                return;
            }
            if (ArrivalDate <= DepartureDate)
            {
                MessageBox.Show("Дата повернення не може бути раніше або дорівнювати даті вильоту!");
                return;
            }

            if (DepartureDate < DateTime.Now.Date)
            {
                MessageBox.Show("Дата вильоту не може бути в минулому!");
                return;
            }
            if (TotalSeats <= 0)
            {
                MessageBox.Show("Кількість місць повинна бути більшою за нуль!");
                return;
            }

            if (TotalSeats > 1000)
            {
                MessageBox.Show("Занадто велика кількість місць! Перевірте дані.");
                return;
            }
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(baseDirectory, "Images", "Tours");

                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);

                string extension = Path.GetExtension(PhotoPath);
                string fileName = $"{Guid.NewGuid()}{extension}";
                string destinationPath = Path.Combine(imagesFolder, fileName);

                File.Copy(PhotoPath, destinationPath, true);
                string relativePath = Path.Combine("Images", "Tours", fileName);

                using (var db = new AppDbContext())
                {
                    var newTour = new Tour
                    {
                        Name = TourName,
                        Description = Description,
                        Photo = relativePath,
                        DepartureDate = DepartureDate,
                        ArrivalDate = ArrivalDate,
                        TotalSeats = TotalSeats,
                        IDHotel = SelectedHotel.ID,
                        IDFood = SelectedFood.ID,
                        IDType = SelectedTourType.ID
                    };
                    db.Tours.Add(newTour);
                    db.SaveChanges();
                }
                MessageBox.Show("Тур успішно створено!");
                var window = Application.Current.Windows.OfType<AddTourWindow>().FirstOrDefault();
                if (window != null)
                {
                    window.DialogResult = true; 
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}