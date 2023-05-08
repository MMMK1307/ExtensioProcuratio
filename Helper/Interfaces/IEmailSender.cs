using ExtensioProcuratio.Helper.Models;

namespace ExtensioProcuratio.Helper.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(EmailModel email);
    }
}
