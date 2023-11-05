using ExtensioProcuratio.App.Project.Queries.CountByUser;
using ExtensioProcuratio.Areas.Identity.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExtensioProcuratio.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISender _mediator;

        public BaseController(UserManager<ApplicationUser> userManager, ISender mediator)
        {
            _userManager = userManager;
            _mediator = mediator;
        }

        private readonly Dictionary<string, int> _userRolesLimits = new()
        {
            {"admin", 10 },
            {"teacher", 8 },
            {"student", 3 },
            {"default", 0 }
        };

        protected bool IsUserAdmin() => User.IsInRole("admin");

        protected bool IsUserTeacher() => User.IsInRole("teacher");

        protected bool IsUserStudent() => User.IsInRole("student");

        protected async Task<string> GetUserId() => (await GetUser()).Id;

        protected async Task<ApplicationUser> GetUser() => await _userManager.GetUserAsync(User);

        protected string UserRole()
        {
            var role = ((ClaimsIdentity)User.Identity!).Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role);

            if (role is null)
            {
                return "default";
            }

            return role.Value;
        }

        protected async Task<bool> IsUserWithinProjectLimit()
        {
            return await _mediator.Send(new CountProjectByUserQuery(await GetUserId())) < _userRolesLimits.GetValueOrDefault(UserRole(), 3);
        }
    }
}