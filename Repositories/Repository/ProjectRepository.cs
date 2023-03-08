using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Data;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExtensioProcuratio.Repositories.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DatabaseContext _databaseContext;

        public ProjectRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<IEnumerable<ProjectModel>> List()
        {
            using (var context = _databaseContext)
            {
                var projectList = await (from proj in context.Project
                                         join user in context.Users on proj.UserId equals user.Id
                                         select new ProjectModel
                                         {
                                             Id = proj.Id,
                                             Name = proj.Name,
                                             Description = proj.Description,
                                             UserId = proj.UserId,
                                             Status = proj.Status,
                                             DateCreated = proj.DateCreated,
                                             DateUpdated = proj.DateUpdated,
                                             ParentName = user.FirstName + " " + user.LastName,
                                             ParentEmail = user.Email
                                         }).AsNoTracking().ToListAsync();
                return projectList;
            }
        }

        public async Task Create(ProjectModel project)
        {
            await _databaseContext.Project.AddAsync(project);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task Update(ProjectModel project)
        {
            _databaseContext.Project.Update(project);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task Delete(ProjectModel project)
        {
            _databaseContext.Project.Remove(project);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<ProjectModel> ListProjectById(string userId)
        {
            return await _databaseContext.Project.AsNoTracking().Where(x => x.Id == userId).FirstOrDefaultAsync() ?? new ProjectModel();
        }

        public async Task<IEnumerable<ProjectModel>> ListUserProjects(string userId)
        {
            using (var context = _databaseContext)
            {
                var projectList = await (from proj in context.Project
                                         join user in context.Users on proj.UserId equals user.Id
                                         where user.Id == userId
                                         select new ProjectModel
                                         {
                                             Id = proj.Id,
                                             Name = proj.Name,
                                             Description = proj.Description,
                                             UserId = proj.UserId,
                                             Status = proj.Status,
                                             DateCreated = proj.DateCreated,
                                             DateUpdated = proj.DateUpdated,
                                             ParentName = user.FirstName + " " + user.LastName,
                                             ParentEmail = user.Email
                                         }).AsNoTracking().ToListAsync();
                return projectList;
            }
        }
    }
}
