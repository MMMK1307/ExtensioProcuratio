using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Data;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExtensioProcuratio.Repositories.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseContext _dbContext;

        public UserRepository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateRole(RolesModel roles)
        {
            var userRoles = new IdentityUserRole<string>()
            {
                UserId = roles.UserId,
                RoleId = roles.RoleId
            };

            await _dbContext.UserRoles.AddAsync(userRoles);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateUser(ApplicationUser user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUser(ApplicationUser user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(c => c.Id == id) ?? new ApplicationUser();
        }

        public async Task<IEnumerable<ApplicationUser>> ListUsersByRole(string roleId)
        {
            using var context = _dbContext;

            var users = await (from user in context.Users
                               join roles in context.UserRoles on user.Id equals roles.UserId
                               where roles.RoleId == roleId
                               select new ApplicationUser
                               {
                                   Id = user.Id,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Email = user.Email,
                                   Subject = user.Subject
                               }).AsNoTracking().ToListAsync();
            return users;
        }
    }
}