using ExtensioProcuratio.Helper;
using ExtensioProcuratio.Helper.Interfaces;
using MediatR;
using System.Text.Encodings.Web;

namespace ExtensioProcuratio.App.Email.ResetPassword
{
    public class ResetPasswordEmailHandler : IRequestHandler<ResetPasswordEmailCommand, bool>
    {
        private readonly IEmailSender _emailSender;
        private readonly UserHelper _userHelper;
        private readonly IHttpHelper _httpHelper;

        public ResetPasswordEmailHandler(IEmailSender emailSender, UserHelper userHelper, IHttpHelper httpHelper)
        {
            _emailSender = emailSender;
            _userHelper = userHelper;
            _httpHelper = httpHelper;
        }

        public async Task<bool> Handle(ResetPasswordEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userHelper.GetUserByEmail(request.Email);

            if (user is null || user.EmailConfirmed == false)
            {
                return false;
            }

            var code = await _userHelper.ResetPasswordTokenUrlEncode(user);

            var callbackUrl = _httpHelper.ResetPasswordEndpoint(code);

            await _emailSender.SendEmailAsync(new Helper.Models.EmailModel(
                request.Email,
                "Reset Senha - Extensio Procuratio",
                $"Olá, para alterar sua senha clique <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>"));

            return true;
        }
    }
}