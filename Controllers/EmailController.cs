using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace ExtensioProcuratio.Controllers
{
    public class EmailController : BaseController
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;

        public EmailController(
            IEmailSender mailSender, UserManager<ApplicationUser> userManager,
            IProjectRepository projectRepository,IUserRepository userRepository)
            :base(projectRepository, userManager)
        {
            _emailSender = mailSender;
            _userManager = userManager;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        public IActionResult ConfirmationEmail() => View();

        public async Task<IActionResult> RetryConfirmationEmail(string retryEmail)
        {
            var user = await _userManager.FindByNameAsync(retryEmail);

            if (user is null || user.EmailConfirmed == true)
            {
                TempData["EmailSenderResponse"] =
                    $"Tens certeza que o e-mail está correto? '{retryEmail}' não existe ou já foi confirmado";

                return RedirectToAction(nameof(ConfirmationEmail));
            }

            var userId = user.Id;
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var mailSentSuccessfully = await SendConfirmEmail(code, userId, retryEmail);

            if (!mailSentSuccessfully)
            {
                TempData["EmailSenderResponse"] =
                    "Tivemos Algum problema para enviar o e-mail. Espere alguns momentos ou reclame com o desenvolvedor \nCOD: SentError";
                return RedirectToAction(nameof(ConfirmationEmail));
            }

            TempData["EmailSenderResponse"] =
                            $"O email foi enviado com sucesso para '{user.Email}'. Caso não esteja em seu inbox, verifique a caixa de spam e/ou tente novamente";

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
            
            var user = await _userRepository.GetUserById(userId);

            user.EmailConfirmed = true;

            await _userRepository.UpdateUser(user);

            TempData["EmailSenderResponse"] = $"Email '{user.Email}' de '{user.FirstName}' foi confirmado";

            return RedirectToAction(nameof(ConfirmationEmail));
        }

        public async Task<IActionResult> SendAdoptionRequest(string userId, ProjectId projectId)
        {
            var teacher = await _userRepository.GetUserById(userId);

            var emailSent = await SendAdoptionEmail(userId, teacher.Email, projectId);

            await _projectRepository.AddAssociateUser(new ProjectAssociatesModel(userId, projectId));

            if (!emailSent)
            {
                return NotFound();
            }

            return RedirectToAction("MyProjects", "Project");
        }

        public async Task<IActionResult> SendProjectSuggestion(ProjectId projectId, string feedback)
        {
            var project = await _projectRepository.ListProjectById(projectId);
            var teacher = await GetUser();

            if (string.IsNullOrEmpty(project.ParentEmail))
            {
                return RedirectToAction("MyProjects", "Project");
            }
            
            feedback = $"Seu projeto {project.Name} recebeu feedback de {teacher.FirstName} {teacher.LastName}.\n" +
                $"Feedback: {feedback}";

            var emailSent = await SendAdoptionFeedbackEmail(project.ParentEmail, feedback);

            if (!emailSent)
            {
                return RedirectToAction("MyProjects", "Project");
            }

            await _projectRepository.RemoveAssociateUsers(new ProjectAssociatesModel(teacher.Id, project.Id));

            return RedirectToAction("MyProjects", "Project");
        }

        public async Task<IActionResult> SendAcceptAdoption(ProjectId projectId)
        {
            var project = await _projectRepository.ListProjectById(projectId);
            var teacher = await GetUser();

            if (string.IsNullOrEmpty(project.ParentEmail))
            {
                return RedirectToAction("MyProjects", "Project");
            }

            var feedback = $"Seu projeto {project.Name} agora conta com a participação de: {teacher.FirstName} {teacher.LastName}.";

            var emailSent = await SendAdoptionFeedbackEmail(project.ParentEmail, feedback);

            if (!emailSent)
            {
                return RedirectToAction("MyProjects", "Project");
            }

            return RedirectToAction("MyProjects", "Project");
        }



        private async Task<bool> SendConfirmEmail(string code, string userId, string email)
        {
            string returnUrl = "";
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl },
                protocol: Request.Scheme);

            if (callbackUrl is null)
            {
                return false;
            }

            var emailContent = new EmailModel(email, "ExtensionProcuration Confirme seu E-mail",
                $"Olá jovem, confirme seu e-mail clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            return await _emailSender.SendEmailAsync(emailContent);
        }   
        
        private async Task<bool> SendAdoptionEmail(string userId, string userEmail, ProjectId projectId)
        {
            var callbackUrl = Url.ActionLink(
                "AdoptionConfirmation", "Project",
                values: new { projectId });

            if (callbackUrl is null)
            {
                return false;
            }

            var emailContent = new EmailModel(userEmail, "ExtensionProcuration Novo Projeto",
                $"Olá professor, uma novo projeto foi proposto a vossa pessoa! Clique <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            return await _emailSender.SendEmailAsync(emailContent);
        }

        private async Task<bool> SendAdoptionFeedbackEmail(string userEmail, string feedback)
        {

            var emailContent = new EmailModel(userEmail, "Seu projeto recebeu Feedback",
                $"Olá jovem! {feedback}");

            return await _emailSender.SendEmailAsync(emailContent);
        }
    }
}