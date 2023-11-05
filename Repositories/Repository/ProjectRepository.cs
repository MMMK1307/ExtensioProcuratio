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

        private IQueryable<ProjectModel> ProjectQuery() => (from proj in _databaseContext.Project
                                                            join user in _databaseContext.Users on proj.UserId equals user.Id
                                                            join associates in _databaseContext.ProjectAssociates on proj.Id equals associates.ProjectId
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
                                                                Subject = proj.Subject,
                                                                Type = proj.Type,
                                                                Participants = proj.Participants,
                                                                DateCreated = proj.DateCreated,
                                                                DateUpdated = proj.DateUpdated,
                                                                ParentName = user.FirstName + " " + user.LastName,
                                                                ParentEmail = user.Email
                                                            }).AsNoTracking();

        public async Task<IEnumerable<ProjectModel>> List()
        {
            return await ProjectQuery().ToListAsync();
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
            project.Status = ProjectStatus.Hidden;
            _databaseContext.Project.Update(project);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<ProjectModel> ListProjectById(ProjectId projectId)
        {
            return await ProjectQuery().Where(x => x.Id == projectId).FirstOrDefaultAsync() ?? new ProjectModel();
        }

        public async Task<IEnumerable<ProjectModel>> ListUserProjects(string userId)
        {
            var query = (from proj in _databaseContext.Project
                         join user in _databaseContext.Users on proj.UserId equals user.Id
                         join associates in _databaseContext.ProjectAssociates on proj.Id equals associates.ProjectId
                         where proj.Status != ProjectStatus.Hidden && associates.UserId == userId
                         select new ProjectModel
                         {
                             Id = proj.Id,
                             Name = proj.Name,
                             Description = proj.Description,
                             UserId = proj.UserId,
                             Status = proj.Status,
                             Edital = proj.Edital,
                             Bolsa = proj.Bolsa,
                             Subject = proj.Subject,
                             Type = proj.Type,
                             Participants = proj.Participants,
                             DateCreated = proj.DateCreated,
                             DateUpdated = proj.DateUpdated,
                             ParentName = user.FirstName + " " + user.LastName,
                             ParentEmail = user.Email
                         }).AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<int> CountUserProjects(string userId)
        {
            return await _databaseContext.Project.AsNoTracking()
                .Where(x => x.UserId == userId && x.Status != ProjectStatus.Hidden).CountAsync();
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

        public async Task<IEnumerable<string>> ListProjectAssociates(ProjectId projectId)
        {
            var users = _databaseContext.ProjectAssociates
                .AsQueryable().AsNoTracking().Where(c => c.ProjectId == projectId);

            return await users.Select(c => c.UserId).ToListAsync();
        }
    }
}