using Microsoft.EntityFrameworkCore;
using CardAccessControl.Models;

namespace CardAccessControl.Data
{
    public class AccessControlContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<AccessTime> AccessTimes { get; set; }
        public DbSet<CardReader> CardReaders { get; set; }

        // A static flag to ensure the database is created only once per application instance.
        private static bool _created = false;

        public AccessControlContext()
        {
            if (!_created)
            {
                Database.EnsureCreated();
                _created = true;
                SeedSampleData();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // SQLite database file named "accesscontrol.db"
            optionsBuilder.UseSqlite("Data Source=accesscontrol.db");
        }

        private void SeedSampleData()
        {
            if (Users.Any()) return;

            var users = new[]
            {
                new User  { Name = "Alice",
                            Cards = new List<Card> { new Card { CardNumber = "0001", AccessLevel = 100 } } },

                new User  { Name = "Bob",
                            Cards = new List<Card> { new Card { CardNumber = "0002", AccessLevel = 10 } } },

                new User  { Name = "Charlie",
                            Cards = new List<Card> { new Card { CardNumber = "0003", AccessLevel = 10 } } }
            };
            Users.AddRange(users);

            var readers = new[]
            {
                new CardReader { Location = "Main Entrance", AccessLevel = 1 },
                new CardReader { Location = "Server Room",   AccessLevel = 5 },
                new CardReader { Location = "Laboratory",    AccessLevel = 3 }
            };
            CardReaders.AddRange(readers);

            SaveChanges();

            AccessTimes.AddRange(new[]
            {
                new AccessTime { Time = DateTime.Now.AddMinutes(-30),
                                 CardId = users[0].Cards.First().Id,
                                 CardReaderId = readers[0].Id },

                new AccessTime { Time = DateTime.Now.AddMinutes(-20),
                                 CardId = users[1].Cards.First().Id,
                                 CardReaderId = readers[1].Id },

                new AccessTime { Time = DateTime.Now.AddMinutes(-10),
                                 CardId = users[2].Cards.First().Id,
                                 CardReaderId = readers[2].Id }
            });

            SaveChanges();
            Console.WriteLine("Sample data inserted into accesscontrol.db");
        }
    }
}
