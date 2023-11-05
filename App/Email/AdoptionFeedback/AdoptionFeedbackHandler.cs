using ExtensioProcuratio.App.Project.Queries.GetById;
using ExtensioProcuratio.App.User.Queries.GetById;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using MediatR;

namespace ExtensioProcuratio.App.Email.AdoptionFeedback
{
    public class AdoptionFeedbackHandler : IRequestHandler<AdoptionFeedbackCommand, (bool, string)>
    {
        private readonly ISender _mediator;
        private readonly IEmailSender _emailSender;

        public AdoptionFeedbackHandler(ISender mediator, IEmailSender emailSender)
        {
            _mediator = mediator;
            _emailSender = emailSender;
        }

        public async Task<(bool, string)> Handle(AdoptionFeedbackCommand request, CancellationToken cancellationToken)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(request.ProjectId), cancellationToken);
            var teacher = await _mediator.Send(new GetUserByIdQuery(request.UserId), cancellationToken);

            if (project is null || project.ParentEmail is null)
            {
                return (false, "Projeto não foi encontrado");
            }

            var feedback = $"Seu projeto {project.Name} recebeu feedback de {teacher.FirstName} {teacher.LastName}.\n" +
                $"Feedback: {request.Feedback}";

            var emailContent = new EmailModel(project.ParentEmail, "Seu projeto recebeu Feedback", feedback);

            var emailSent = await _emailSender.SendEmailAsync(emailContent);

            if (!emailSent)
            {
                return (false, "Não foi possível enviar");
            }

            return (true, "Email enviado com sucesso!");
        }
    }
}