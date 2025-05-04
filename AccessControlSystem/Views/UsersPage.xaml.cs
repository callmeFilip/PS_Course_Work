using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AccessControlSystem.Data;
using AccessControlSystem.Models;

namespace AccessControlSystem.Views
{
    public partial class UsersPage : Page
    {
        private readonly IUnitOfWork _uow;
        private readonly ObservableCollection<User> _users = new();

        public UsersPage(IUnitOfWork uow)
        {
            InitializeComponent();
            _uow = uow;
            UsersGrid.ItemsSource = _users;
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var list = await _uow.Users.GetAllAsync();
            _users.Clear();
            foreach (var u in list) _users.Add(u);
        }

        private async void Add_Click(object s, RoutedEventArgs e)
        {
            var name = UserNameBox.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;

            var user = new User { Name = name };
            await _uow.Users.AddAsync(user);
            await _uow.CommitAsync();

            _users.Add(user);
            UserNameBox.Clear();
        }

        private async void Delete_Click(object s, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is not User u) return;

            if (MessageBox.Show($"Delete '{u.Name}' and ALL their cards & access logs?",
                                "Confirm",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes)
            { 
                return;
            }

            _uow.Users.Remove(_uow.Users.GetTrackedOrAttach(u));
            await _uow.CommitAsync();
            _users.Remove(u);
        }

        private void Back_Click(object s, RoutedEventArgs e) =>
            NavigationService?.GoBack();
    }
}
