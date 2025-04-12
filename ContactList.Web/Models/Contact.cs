using System.ComponentModel.DataAnnotations;

namespace ContactList.Web.Models
{

    // Models/Contact.cs
    public class Contact
    {
        public int ContactId { get; set; }
        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Notes { get; set; }
    }

}
