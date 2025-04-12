using ContactList.Web.Models;
using ContactList.Web.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContactList.Tests.Services
{
    [TestFixture]
    public class ContactServiceTests
    {
        private AppDbContext _context;
        private IContactService _contactService;
        private List<Contact> _contacts;

        [SetUp]
        public void SetUp()
        {
            // Set up In-Memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test run
                .Options;

            _context = new AppDbContext(options);

            // Seed some sample contacts into the in-memory database, including required properties
            _contacts = new List<Contact>
            {
                new Contact { ContactId = 1, Name = "Alice", UserId = 1, Email = "alice@example.com", Notes = "Friend", Phone = "123-456-7890" },
                new Contact { ContactId = 2, Name = "Bob", UserId = 1, Email = "bob@example.com", Notes = "Work", Phone = "234-567-8901" },
                new Contact { ContactId = 3, Name = "Charlie", UserId = 2, Email = "charlie@example.com", Notes = "Family", Phone = "345-678-9012" }
            };

            _context.Contacts.AddRange(_contacts);
            _context.SaveChanges();

            // Initialize ContactService with real DbContext
            _contactService = new ContactService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of the context after each test
            _context.Dispose();
        }

        [Test]
        public void AddContact_AddsAndSavesContact()
        {
            var newContact = new Contact
            {
                ContactId = 4,
                Name = "Dave",
                UserId = 1,
                Email = "dave@example.com",  // Add required properties
                Notes = "New Contact",       // Add required properties
                Phone = "456-789-0123"       // Add required properties
            };

            _contactService.AddContact(newContact);

            var addedContact = _context.Contacts.Find(newContact.ContactId);
            Assert.IsNotNull(addedContact);
            Assert.AreEqual(newContact.Name, addedContact.Name);
            Assert.AreEqual(newContact.Email, addedContact.Email);
            Assert.AreEqual(newContact.Phone, addedContact.Phone);
            Assert.AreEqual(newContact.Notes, addedContact.Notes);
        }

        [Test]
        public void GetContacts_NoSearch_ReturnsContactsForUser()
        {
            var result = _contactService.GetContacts(1);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(c => c.UserId == 1));
        }

        [Test]
        public void GetContacts_WithSearch_ReturnsFilteredContacts()
        {
            var result = _contactService.GetContacts(1, "Bob");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Bob", result[0].Name);
        }

        [Test]
        public void GetContact_ValidId_ReturnsContact()
        {
            var contact = _contacts.First();

            var result = _contactService.GetContact(contact.ContactId);

            Assert.AreEqual(contact, result);
        }

        [Test]
        public void UpdateContact_UpdatesAndSavesContact()
        {
            var updatedContact = new Contact
            {
                ContactId = 1, // Same ID as the contact to update
                Name = "Updated Alice",
                UserId = 1,
                Email = "alice.updated@example.com",
                Notes = "Updated",
                Phone = "987-654-3210"
            };

            // Ensure the existing contact is detached before updating
            var existingContact = _context.Contacts.Find(updatedContact.ContactId);
            if (existingContact != null)
            {
                _context.Entry(existingContact).State = EntityState.Detached; // Detach the existing entity
            }

            // Now update the contact
            _contactService.UpdateContact(updatedContact);

            var result = _context.Contacts.Find(updatedContact.ContactId);

            // Verify the updated contact's values
            Assert.AreEqual("Updated Alice", result?.Name);
            Assert.AreEqual("alice.updated@example.com", result?.Email);
            Assert.AreEqual("Updated", result?.Notes);
            Assert.AreEqual("987-654-3210", result?.Phone);
        }


        [Test]
        public void DeleteContact_ExistingContact_RemovesAndSaves()
        {
            var contact = _contacts.First();

            _contactService.DeleteContact(contact.ContactId);

            var deletedContact = _context.Contacts.Find(contact.ContactId);
            Assert.IsNull(deletedContact);
        }

        [Test]
        public void DeleteContact_NonExistingContact_DoesNothing()
        {
            var initialCount = _context.Contacts.Count();

            _contactService.DeleteContact(99); // ID that doesn't exist

            var countAfterDelete = _context.Contacts.Count();
            Assert.AreEqual(initialCount, countAfterDelete); // No change should happen
        }
    }
}
