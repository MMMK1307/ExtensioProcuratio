using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Models;

namespace ExtensioProcuratio.Helper.Models
{
    public class HttpHelper : IHttpHelper
    {
        public readonly IHttpContextAccessor _httpContextAccessor;

        public HttpHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Host => "https://" + _httpContextAccessor.HttpContext!.Request.Host;

        public string ConfirmEmailEndpoint(string userId, string confirmEmailCode) => Host + "/Identity/Account/ConfirmEmail?userId=" + userId + "&code=" + confirmEmailCode;

        public string ConfirmEmailEndpoint(string userId, string confirmEmailCode, string returnUrl) => Host + "/Identity/Account/ConfirmEmail?userId=" + userId + "&code=" + confirmEmailCode + "&returnUrl=" + returnUrl;

        public string ConfirmEmailByAdminEndpoint(string userId) => Host + "/Email/ConfirmEmail?userId=" + userId;

        public string AdoptionConfirmationEndpoint(ProjectId projectId) => Host + "/Project/AdoptionConfirmation?projectId=" + projectId;

        public string ResetPasswordEndpoint(string code) => Host + "/Identity/Account/ResetPassword?code=" + code;
    }
}