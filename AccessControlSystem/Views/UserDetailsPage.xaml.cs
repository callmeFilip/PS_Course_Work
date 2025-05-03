using System.Threading.Tasks;
using System.Windows.Controls;
using AccessControlSystem.Data;
using AccessControlSystem.Models;

namespace AccessControlSystem.Views
{
    public partial class UserDetailsPage : Page
    {
        private readonly IUnitOfWork _uow;

        // properties bound in XAML
        public int Id { get; private set; }
        public string UserName { get; private set; } = "";
        public IEnumerable<Card> Cards { get; private set; } = Enumerable.Empty<Card>();

        public UserDetailsPage(IUnitOfWork uow, int cardId)
        {
            InitializeComponent();
            _uow = uow;
            _ = LoadAsync(cardId);
        }

        private async Task LoadAsync(int cardId)
        {
            var card = await _uow.Cards.GetByIdAsync(cardId);
            if (card?.UserId is int userId)
            {
                var user = await _uow.Users.GetByIdAsync(userId);
                if (user is not null)
                {
                    Id = user.Id;
                    UserName = user.Name;                    // ← assign
                    Cards = await _uow.Cards.GetCardsForUserAsync(userId);
                    DataContext = this;                       // bindings refresh
                }
            }
        }

        private void Back_Click(object sender, System.Windows.RoutedEventArgs e) =>
            NavigationService?.GoBack();
    }
}
