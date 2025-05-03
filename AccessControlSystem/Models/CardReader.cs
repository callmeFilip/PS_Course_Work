using System.Collections.Generic;

namespace CardAccessControl.Models
{
    public class CardReader
    {
        public int Id { get; set; }
        public string Location { get; set; }
        // Level of access - bigger integer means higher level of access
        public int AccessLevel { get; set; }

        // Navigation property: a card reader logs multiple access times.
        public ICollection<AccessTime> AccessTimes { get; set; }
    }
}
