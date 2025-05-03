using System.Collections.Generic;

namespace CardAccessControl.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public int AccessLevel { get; set; }

        // Foreign key to User
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation property: a card can have multiple access times.
        public ICollection<AccessTime> AccessTimes { get; set; }
    }
}
