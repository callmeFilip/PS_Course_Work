using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AccessControlSystem.Data;
using AccessControlSystem.Models;

namespace AccessControlSystem.Views
{
    public partial class ReadersPage : Page
    {
        private readonly IUnitOfWork _uow;
        private readonly ObservableCollection<CardReader> _readers = new();

        public ReadersPage(IUnitOfWork uow)
        {
            InitializeComponent();
            _uow = uow;
            ReadersGrid.ItemsSource = _readers;
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var list = await _uow.CardReaders.GetAllAsync();
            _readers.Clear();
            foreach (var r in list) _readers.Add(r);
        }

        private async void Add_Click(object s, RoutedEventArgs e)
        {
            if (!int.TryParse(LevelBox.Text, out int lvl)) return;
            var loc = LocationBox.Text.Trim();
            if (string.IsNullOrEmpty(loc)) return;

            var reader = new CardReader { Location = loc, AccessLevel = lvl };
            await _uow.CardReaders.AddAsync(reader);
            await _uow.CommitAsync();

            _readers.Add(reader);
            LocationBox.Clear(); LevelBox.Clear();
        }

        private async void Delete_Click(object s, RoutedEventArgs e)
        {
            if (ReadersGrid.SelectedItem is not CardReader r) return;

            if (MessageBox.Show($"Delete reader '{r.Location}'?", "Confirm",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            _uow.CardReaders.Remove(r);
            await _uow.CommitAsync();
            _readers.Remove(r);
        }

        private void Back_Click(object s, RoutedEventArgs e) =>
            NavigationService?.GoBack();
    }
}
