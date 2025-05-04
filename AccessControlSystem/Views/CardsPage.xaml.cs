// Views/CardsPage.xaml.cs
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AccessControlSystem.Data;
using AccessControlSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Views
{
    public partial class CardsPage : Page
    {
        private readonly IUnitOfWork _uow;
        private readonly ObservableCollection<Card> _cards = new();

        public CardsPage(IUnitOfWork uow)
        {
            InitializeComponent();
            _uow = uow;

            CardsGrid.ItemsSource = _cards;
            _ = LoadAsync();
        }


        private async Task LoadAsync()
        {
            // Owners for ComboBox
            var users = (await _uow.Users.GetAllAsync())
                        .OrderBy(u => u.Name)
                        .ToList();

            UserBox.ItemsSource = users;
            if (users.Count > 0) UserBox.SelectedIndex = 0;

            // Cards with owner already loaded
            var list = await _uow.Cards.GetAllWithOwnerAsync();

            _cards.Clear();
            foreach (var c in list) _cards.Add(c);
        }
        private async void Add_Click(object s, RoutedEventArgs e)
        {
            if (UserBox.SelectedValue is not int userId) return;

            var number = CardNumberBox.Text.Trim();
            if (string.IsNullOrEmpty(number)) return;

            if (!int.TryParse(CardLevelBox.Text, out int level)) return;

            var owner = _uow.Users.GetTrackedOrAttach(new User { Id = userId });

            var card = new Card
            {
                CardNumber = number,
                AccessLevel = level,
                User = owner,
                UserId = userId
            };

            await _uow.Cards.AddAsync(card);
            await _uow.CommitAsync();

            // Load owner navigation for grid display
            _cards.Add(card);

            CardNumberBox.Clear(); CardLevelBox.Clear();
        }

        private async void Delete_Click(object s, RoutedEventArgs e)
        {
            if (CardsGrid.SelectedItem is not Card card) return;

            if (MessageBox.Show($"Delete card '{card.CardNumber}' ?",
                                "Confirm", MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) != MessageBoxResult.Yes) return;

            _uow.Cards.Remove(_uow.Cards.GetTrackedOrAttach(card));
            await _uow.CommitAsync();
            _cards.Remove(card);
        }

        private void Back_Click(object s, RoutedEventArgs e) =>
            NavigationService?.GoBack();
    }
}
