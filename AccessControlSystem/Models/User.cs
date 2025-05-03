using System.Collections.Generic;

namespace AccessControlSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // A user can have multiple cards
        public ICollection<Card> Cards { get; set; }
    }
}