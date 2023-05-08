using ExtensioProcuratio.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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
    }
}