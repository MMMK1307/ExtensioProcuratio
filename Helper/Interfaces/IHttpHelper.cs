using ExtensioProcuratio.Models;

namespace ExtensioProcuratio.Helper.Interfaces
{
    public interface IHttpHelper
    {
        public string Host { get; }

        public string ConfirmEmailEndpoint(string userId, string confirmEmailCode);

        public string ConfirmEmailEndpoint(string userId, string confirmEmailCode, string returnUrl);

        public string ConfirmEmailByAdminEndpoint(string userId);

        public string AdoptionConfirmationEndpoint(ProjectId projectId);

        public string ResetPasswordEndpoint(string code);
    }
}