#nullable disable

using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Controllers;
using ExtensioProcuratio.Enumerators;
using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Helper.Models;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace ExtensioProcuratio.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _userRepository = userRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Subject")]
            public SubjectAreas Subject { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "As senhas não parecem iguais.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Subject = Input.Subject;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _userRepository.CreateRole(new RolesModel() { RoleId = Input.Role, UserId = userId });

                    bool emailSentSuccesfully = false;

                    emailSentSuccesfully = Input.Role switch
                    {
                        "1" => false,
                        "2" => await SendTeacherConfirmEmail(userId, $"{user.FirstName} {user.LastName} ({user.Email})"),
                        _ => await SendConfirmEmail(code, userId, returnUrl),
                    };

                    if (!emailSentSuccesfully)
                    {
                        TempData["EmailSenderResponse"] =
                            "Tivemos Algum problema para enviar o e-mail. Espere algums momentos ou reclame com o desenvolvedor \nCOD: SentError";

                        return RedirectToAction(nameof(EmailController.ConfirmationEmail), "Email");
                    }
                    
                    TempData["EmailSenderResponse"] =
                            $"O email foi enviado com sucesso para {Input.Email}. Caso não esteja em seu inbox, verifique a caixa de spam e/ou tente novamente";

                    if (Input.Role == "2")
                    {
                        TempData["EmailSenderResponse"] =
                            $"O email foi enviado com sucesso para um Admin. Por favor aguarde um tempinho (na verdade muito tempo)";
                    }


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction(nameof(EmailController.ConfirmationEmail), "Email");
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private async Task<bool> SendConfirmEmail(string code, string userId, string returnUrl)
        {
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);

            var emailContent = new EmailModel(Input.Email, "ExtensionProcuration Confirme seu E-mail",
                $"Olá jovem, confirme seu e-mail clicando <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            return await _emailSender.SendEmailAsync(emailContent);
        }

        private async Task<bool> SendTeacherConfirmEmail(string userId, string name)
        {
            var callbackUrl = Url.ActionLink(
                "ConfirmEmail","Email",
                values: new { userId });

            var adminEmail = "mikael.strapasson1@gmail.com";

            var emailContent = new EmailModel(adminEmail, "ExtensionProcuration. Confirmação Professor",
                $"Olá admin. Professor {name} precisa confirmar sua conta. Clique <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>aqui</a>.");

            return await _emailSender.SendEmailAsync(emailContent);
        }
    }
}