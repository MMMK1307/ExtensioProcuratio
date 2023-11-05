using ExtensioProcuratio.App.Email.AdoptionFeedback;
using ExtensioProcuratio.App.Email.AdoptionRequest;
using ExtensioProcuratio.App.Email.EmailConfirmation;
using ExtensioProcuratio.App.ProjectAssociates.Commands.Remove;
using ExtensioProcuratio.App.User.Commands.Update;
using ExtensioProcuratio.App.User.Queries.GetById;
using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExtensioProcuratio.Controllers
{
    public class EmailController : BaseController
    {
        private readonly ISender _mediator;

        public EmailController(
            UserManager<ApplicationUser> userManager,
            ISender mediator)
            : base(userManager, mediator)
        {
            _mediator = mediator;
        }

        public IActionResult ConfirmationEmail() => View();

        public async Task<IActionResult> RetryConfirmationEmail(string retryEmail)
        {
            var (Result, Message) = await _mediator.Send(new EmailConfirmationCommand(retryEmail, ""));

            TempData["EmailSenderResponse"] = Message;

            return RedirectToAction(nameof(ConfirmationEmail));
        }

        public async Task<IActionResult> ConfirmEmail(string userId)
        {
            if (!IsUserAdmin())
            {
                TempData["EmailSenderResponse"] =
                    "Tivemos Algum problema para enviar o e-mail. Espere alguns momentos ou reclame com o desenvolvedor \nCOD: IncompatibleRole";
                return RedirectToAction(nameof(ConfirmationEmail));
            }

            var user = await _mediator.Send(new GetUserByIdQuery(userId));
            user.EmailConfirmed = true;
            await _mediator.Send(new UpdateUserCommand(user));

            TempData["EmailSenderResponse"] = $"Email '{user.Email}' de '{user.FirstName}' foi confirmado";

            return RedirectToAction(nameof(ConfirmationEmail));
        }

        public async Task<IActionResult> SendAdoptionRequest(string userId, ProjectId projectId)
        {
            var (Result, Message) = await _mediator.Send(new AdoptionRequestCommand(projectId, userId));

            if (!Result)
            {
                return NotFound();
            }

            TempData["EmailSenderResponse"] = Message;

            return RedirectToAction("MyProjects", "Project");
        }

        public async Task<IActionResult> SendAdoptionFeedback(ProjectId projectId, string feedback)
        {
            var userId = await GetUserId();

            await _mediator.Send(new AdoptionFeedbackCommand(projectId, userId, feedback));

            await _mediator.Send(new RemoveProjectAssociateCommand(projectId, userId));

            return RedirectToAction("MyProjects", "Project");
        }

        public async Task<IActionResult> SendAcceptAdoption(ProjectId projectId)
        {
            var userId = await GetUserId();

            var feedback = $"Participação confirmada! Entre em contato para mais informações";

            var (Result, _) = await _mediator.Send(new AdoptionFeedbackCommand(projectId, userId, feedback));

            if (!Result)
            {
                return RedirectToAction("MyProjects", "Project");
            }

            return RedirectToAction("MyProjects", "Project");
        }
    }
}