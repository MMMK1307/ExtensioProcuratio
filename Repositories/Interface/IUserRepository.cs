using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Models;

namespace ExtensioProcuratio.Repositories.Interface
{
    public interface IUserRepository
    {
        Task CreateUser(ApplicationUser user);
        Task UpdateUser(ApplicationUser user);
        Task<ApplicationUser> GetUserById(string id);
        Task CreateRole(RolesModel roles);
        Task<IEnumerable<ApplicationUser>> ListUsersByRole(string roleId);
    }
}