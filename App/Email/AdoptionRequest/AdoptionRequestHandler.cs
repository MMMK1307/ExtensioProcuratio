using ExtensioProcuratio.App.ProjectAssociates.Commands.Add;
using ExtensioProcuratio.App.User.Queries.GetById;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using MediatR;
using System.Text.Encodings.Web;

namespace ExtensioProcuratio.App.Email.AdoptionRequest
{
    public class AdoptionRequestHandler : IRequestHandler<AdoptionRequestCommand, (bool, string)>
    {
        private readonly ISender _mediator;
        private readonly IEmailSender _emailSender;
        private readonly IHttpHelper _httpHelper;

        public AdoptionRequestHandler(ISender mediator, IEmailSender emailSender, IHttpHelper httpHelper)
        {
            _mediator = mediator;
            _emailSender = emailSender;
            _httpHelper = httpHelper;
        }

        public async Task<(bool, string)> Handle(AdoptionRequestCommand request, CancellationToken cancellationToken)
        {
            var teacher = await _mediator.Send(new GetUserByIdQuery(request.UserId), cancellationToken);

            var callbackUrl = _httpHelper.AdoptionConfirmationEndpoint(request.ProjectId);

            var emailContent = new EmailModel(teacher.Email, "ExtensioProcuratio Novo Projeto",
                $"Olá professor, uma novo projeto foi proposto a vossa pessoa! Clique <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            var emailSent = await _emailSender.SendEmailAsync(emailContent);

            await _mediator.Send(new AddProjectAssociateCommand(request.ProjectId, request.UserId), cancellationToken);

            if (!emailSent)
            {
                return (false, "Não foi possível enviar o email");
            }

            return (true, "O email foi enviado com Sucesso. Por favor aguarde um feedback do professor selecionado");
        }
    }
}