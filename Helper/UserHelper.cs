using ExtensioProcuratio.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace ExtensioProcuratio.Helper
{
    public class UserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GetName(ClaimsPrincipal mainUser)
        {
            var user = await _userManager.GetUserAsync(mainUser);
            return user.FirstName + " " + user.LastName;
        }

        public async Task<ApplicationUser> GetUserByEmail(string email) => await _userManager.FindByEmailAsync(email);

        public async Task<string> EmailConfirmationToken(ApplicationUser user) => await _userManager.GenerateEmailConfirmationTokenAsync(user);

        public async Task<string> EmailConfirmationTokenUrlEncode(ApplicationUser user) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await EmailConfirmationToken(user)));

        public async Task<string> ResetPasswordToken(ApplicationUser user) => await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<string> ResetPasswordTokenUrlEncode(ApplicationUser user) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await ResetPasswordToken(user)));
    }
}