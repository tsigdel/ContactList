// Services/IAuthService.cs
using ContactList.Web.Models;

namespace ContactList.Web.Services
{
    public interface IAuthService
    {
        User? Authenticate(string username, string password);
        bool Register(string username, string password);
        bool ResetPassword(string username, string newPassword);
    }
}
