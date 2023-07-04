using ExtensioProcuratio.Data;
using ExtensioProcuratio.Enumerators;
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
            var projectListQuery = (from proj in _databaseContext.Project
                                    join user in _databaseContext.Users on proj.UserId equals user.Id
                                    where proj.Status != ProjectStatus.Hidden
                                    select new ProjectModel
                                    {
                                        Id = proj.Id,
                                        Name = proj.Name,
                                        Description = proj.Description,
                                        UserId = proj.UserId,
                                        Status = proj.Status,
                                        Edital = proj.Edital,
                                        Bolsa = proj.Bolsa,
                                        Participants = proj.Participants,
                                        DateCreated = proj.DateCreated,
                                        DateUpdated = proj.DateUpdated,
                                        ParentName = user.FirstName + " " + user.LastName,
                                        ParentEmail = user.Email
                                    }).AsQueryable().AsNoTracking();

            return await projectListQuery.ToListAsync();
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

        public async Task<ProjectModel> ListProjectById(string projectId)
        {
            var project = await (from proj in _databaseContext.Project
                                 join user in _databaseContext.Users on proj.UserId equals user.Id
                                 where proj.Id == projectId
                                 select new ProjectModel
                                 {
                                     Id = proj.Id,
                                     Name = proj.Name,
                                     Description = proj.Description,
                                     UserId = proj.UserId,
                                     Status = proj.Status,
                                     Edital = proj.Edital,
                                     Bolsa = proj.Bolsa,
                                     Participants = proj.Participants,
                                     DateCreated = proj.DateCreated,
                                     DateUpdated = proj.DateUpdated,
                                     ParentName = user.FirstName + " " + user.LastName,
                                     ParentEmail = user.Email
                                 }).AsNoTracking().FirstOrDefaultAsync();

            return project ?? new ProjectModel();
        }

        public async Task<IEnumerable<ProjectModel>> ListUserProjects(string userId)
        {
            var projectList = await (from proj in _databaseContext.Project
                                     join user in _databaseContext.Users on proj.UserId equals user.Id
                                     where user.Id == userId && proj.Status != ProjectStatus.Hidden
                                     select new ProjectModel
                                     {
                                         Id = proj.Id,
                                         Name = proj.Name,
                                         Description = proj.Description,
                                         UserId = proj.UserId,
                                         Status = proj.Status,
                                         Edital = proj.Edital,
                                         Bolsa = proj.Bolsa,
                                         Participants = proj.Participants,
                                         DateCreated = proj.DateCreated,
                                         DateUpdated = proj.DateUpdated,
                                         ParentName = user.FirstName + " " + user.LastName,
                                         ParentEmail = user.Email
                                     }).AsNoTracking().AsQueryable().ToListAsync();
            return projectList;
        }

        public async Task<int> CountUserProjects(string userId)
        {
            return await _databaseContext.Project.AsNoTracking()
                .Where(x => x.UserId == userId).CountAsync();
        }

        public async Task AddAssociateUser(ProjectAssociatesModel associate)
        {
            await _databaseContext.ProjectAssociates.AddAsync(associate);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task RemoveAssociateUsers(ProjectAssociatesModel associate)
        {
            var query = _databaseContext.ProjectAssociates
                .AsQueryable()
                .Where(c => c.ProjectId == associate.ProjectId && c.UserId == associate.UserId);

            var dbAssociates = await query.FirstOrDefaultAsync();

            if (dbAssociates is null)
            {
                return;
            }

            _databaseContext.ProjectAssociates.Remove(dbAssociates);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<string>> ListProjectAssociates(string projectId)
        {
            var users = _databaseContext.ProjectAssociates
                .AsQueryable().AsNoTracking().Where(c => c.ProjectId == projectId);

            return await users.Select(c => c.UserId).ToListAsync();
        }
    }
}