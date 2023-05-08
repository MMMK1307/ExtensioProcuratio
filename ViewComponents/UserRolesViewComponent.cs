using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ExtensioProcuratio.ViewComponents
{
    public class UserRolesViewComponent : ViewComponent
    {
        private readonly IUserRepository _userRepository;

        public UserRolesViewComponent(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(string roleId)
        {
            var users = await _userRepository.ListUsersByRole(roleId);

            return View(users);
        }
    }
}
