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
            mqtt.AccessTimeLogged += (_, at) =>
                Dispatcher.Invoke(() => AccessTimes.Insert(0, at));

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var latest = await _uow.AccessTimes.GetLatestAsync(100);
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
    }
}
