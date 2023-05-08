using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExtensioProcuratio.Controllers
{
    public class BaseController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BaseController(IProjectRepository projectRepository, UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
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

            if(role is null) 
            { 
                return "default"; 
            }

            return role.Value;
        }

        protected async Task<bool> IsUserWithinProjectLimit()
        {
            return await _projectRepository.CountUserProjects(await GetUserId()) < _userRolesLimits[UserRole()];
        }
    }
}
