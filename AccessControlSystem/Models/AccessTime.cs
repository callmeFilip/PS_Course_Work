using System;
using System.Formats.Tar;

namespace CardAccessControl.Models
{
    public class AccessTime
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }

        // Foreign key to Card
        public int CardId { get; set; }
        public Card Card { get; set; }

        // Foreign key to CardReader
        public int CardReaderId { get; set; }
        public CardReader CardReader { get; set; }
    }
}
