using ExtensioProcuratio.Helper;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using MediatR;
using System.Text.Encodings.Web;

namespace ExtensioProcuratio.App.Email.EmailConfirmation
{
    public class EmailConfirmationHandler : IRequestHandler<EmailConfirmationCommand, (bool, string)>
    {
        private readonly IEmailSender _emailSender;
        private readonly IHttpHelper _httpHelper;
        private readonly UserHelper _userHelper;

        public EmailConfirmationHandler(IEmailSender emailSender, IHttpHelper httpHelper, UserHelper userHelper)
        {
            _emailSender = emailSender;
            _httpHelper = httpHelper;
            _userHelper = userHelper;
        }

        public async Task<(bool, string)> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userHelper.GetUserByEmail(request.Email);

            if (user is null || user.EmailConfirmed == true)
            {
                return (false, $"Tens certeza que o e-mail está correto? '{request.Email}' não existe ou já foi confirmado");
            }

            var confirmationToken = await _userHelper.EmailConfirmationTokenUrlEncode(user);

            var callbackUrl = _httpHelper.ConfirmEmailEndpoint(user.Id, confirmationToken);

            var emailContent = new EmailModel(request.Email, "ExtensioProcuratio Confirme seu E-mail",
                $"Olá jovem, confirme seu e-mail clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            var mailSentSuccessfully = await _emailSender.SendEmailAsync(emailContent);

            if (!mailSentSuccessfully)
            {
                return (false, "Tivemos Algum problema para enviar o e-mail. Espere alguns momentos ou reclame com o desenvolvedor \nCOD: SentError");
            }

            return (true, $"O email foi enviado com sucesso para '{user.Email}'. Caso não esteja em seu inbox, verifique a caixa de spam e/ou tente novamente");
        }
    }
}