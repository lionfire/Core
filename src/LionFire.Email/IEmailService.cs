using System.Threading.Tasks;

namespace LionFire.Email
{
    public interface IEmailService
    {
        Task<bool> Send(string from, string to, string subject, string message);
    }

}
