using System.ComponentModel.DataAnnotations;

namespace ContactList.Web.Models
{
    // Models/User.cs
    public class User
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }
    }

}
