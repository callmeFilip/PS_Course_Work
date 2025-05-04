using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AccessControlSystem.Data;
using AccessControlSystem.Models;
using AccessControlSystem.Services;

namespace AccessControlSystem.Views
{
    public partial class AccessLogPage : Page
    {
        private readonly IUnitOfWork _uow;

        public ObservableCollection<AccessTime> AccessTimes { get; } = new();

        public AccessLogPage(IUnitOfWork uow, MqttService mqtt)
        {
            InitializeComponent();
            _uow = uow;
            DataContext = this;

            // live updates
            mqtt.AccessTimeLogged += AccessTimeLoggedAsync;

            _ = LoadAsync();
        }

        private async void AccessTimeLoggedAsync(object? _, AccessTime at)
        {
            // Ensure the navigation property is loaded so the binding engine can resolve the Location column
            at.CardReader ??= await _uow.CardReaders.GetByIdAsync(at.CardReaderId);

            // Marshal to the UI thread since we're updating an ObservableCollection bound to the grid
            Dispatcher.Invoke(() => AccessTimes.Insert(0, at));
        }

        private async Task LoadAsync()
        {
            var latest = await _uow.AccessTimes.GetLatestWithReaderAsync(100);

            Dispatcher.Invoke(() =>
            {
                AccessTimes.Clear();
                foreach (var a in latest) AccessTimes.Add(a);
            });
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not null &&
                (sender as FrameworkElement)?.DataContext is AccessTime at)
            {
                // navigate to details page
                NavigationService?.Navigate(new UserDetailsPage(_uow, at.CardId));
            }
        }

        private void ManageUsers_Click(object s, RoutedEventArgs e) =>
            NavigationService?.Navigate(new UsersPage(_uow));

        private void ManageReaders_Click(object s, RoutedEventArgs e) =>
            NavigationService?.Navigate(new ReadersPage(_uow));
        private void ManageCards_Click(object s, RoutedEventArgs e) =>
            NavigationService?.Navigate(new CardsPage(_uow));

    }
}
