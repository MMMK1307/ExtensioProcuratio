using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using MediatR;
using System.Text.Encodings.Web;

namespace ExtensioProcuratio.App.Email.EmailConfirmationByAdmin
{
    public class EmailConfirmationByAdminHandler : IRequestHandler<EmailConfirmationByAdminCommand, (bool, string)>
    {
        private readonly IEmailSender _emailSender;
        private readonly IHttpHelper _httpHelper;

        public EmailConfirmationByAdminHandler(IEmailSender emailSender, IHttpHelper httpHelper)
        {
            _emailSender = emailSender;
            _httpHelper = httpHelper;
        }

        public async Task<(bool, string)> Handle(EmailConfirmationByAdminCommand request, CancellationToken cancellationToken)
        {
            var callbackUrl = _httpHelper.ConfirmEmailByAdminEndpoint(request.UserId);

            var adminEmail = "mikael.strapasson1@gmail.com";

            var emailContent = new EmailModel(adminEmail, "ExtensioProcuratio. Confirmação Professor",
                $"Olá admin. Professor {request.Name} precisa confirmar sua conta. Clique <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            var emailSent = await _emailSender.SendEmailAsync(emailContent);

            if (!emailSent)
            {
                return (false, "Error ao enviar o email para Admin");
            }

            return (true, "Email enviado com sucesso para um de nossos admins. Por favor aguarde a liberação da sua conta");
        }
    }
}