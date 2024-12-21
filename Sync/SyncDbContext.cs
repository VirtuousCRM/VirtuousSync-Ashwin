using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    internal class SyncDbContext : DbContext
    {
        public SyncDbContext(SQLiteConnection connection) : base(connection, contextOwnsConnection: false) { }

        public DbSet<Contact> Contacts { get; set; }
    }

    internal class SyncContacts
    {
        public void AddContactToDatbase(List<AbbreviatedContact> contacts)
        {
            //Adding contacts to an in-memory SQLite database, since we don't have access to a real database
            var connection = new SQLiteConnection("DataSource=:memory:;Version=3;New=True;");
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Contacts (
                    RowId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Id INTEGER,
                    Name TEXT NOT NULL,
                    ContactType TEXT NOT NULL,
                    ContactName TEXT NOT NULL,
                    Address TEXT NULL,
                    Email TEXT NULL,
                    Phone TEXT NULL
                );";

            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }

            using (var context = new SyncDbContext(connection))
            {
                context.Database.CreateIfNotExists();

                //iterate through each AbbreviatedContact and create a Contact object to insert into Contacts database
                foreach (var contact in contacts)
                {
                    var dbContact = new Contact
                    {
                        Id = contact.Id,
                        Name = contact.Name,
                        ContactType = contact.ContactType,
                        ContactName = contact.ContactName,
                        Address = contact.Address,
                        Email = contact.Email,
                        Phone = contact.Phone
                    };

                    context.Contacts.Add(dbContact);
                    context.SaveChanges();
                }

                foreach (var contact in context.Contacts)
                {
                    Console.WriteLine($"ContactId: {contact.Id}, Name: {contact.Name}, Address: {contact.Address}");
                }
            }
        }
    }
}
