// IContactService.cs
using ContactList.Web.Models;

namespace ContactList.Web.Services
{
    public interface IContactService
    {
        List<Contact> GetContacts(int userId, string? search = null);
        void AddContact(Contact contact);
        Contact? GetContact(int id);
        void UpdateContact(Contact contact);
        void DeleteContact(int id);
    }
}
