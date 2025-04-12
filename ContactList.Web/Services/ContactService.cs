// ContactService.cs
using ContactList.Web.Models;

namespace ContactList.Web.Services
{
    public class ContactService : IContactService
    {
        private readonly AppDbContext _context;
        public ContactService(AppDbContext context)
        {
            _context = context;
        }

        public List<Contact> GetContacts(int userId, string? search = null)
        {
            var query = _context.Contacts.Where(c => c.UserId == userId);
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Name.Contains(search));
            return query.ToList();
        }

        public void AddContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            _context.SaveChanges();
        }

        public Contact? GetContact(int id) => _context.Contacts.Find(id);

        public void UpdateContact(Contact contact)
        {
            _context.Contacts.Update(contact);
            _context.SaveChanges();
        }

        public void DeleteContact(int id)
        {
            var contact = _context.Contacts.Find(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
            }
        }
    }
}
