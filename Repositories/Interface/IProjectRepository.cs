using ExtensioProcuratio.Models;

namespace ExtensioProcuratio.Repositories.Interface
{
    public interface IProjectRepository
    {
        Task<IEnumerable<ProjectModel>> List();
        Task Create(ProjectModel project);
        Task Update(ProjectModel project);
        Task Delete(ProjectModel project);
        Task<ProjectModel> ListProjectById(string projectId);
        Task<IEnumerable<ProjectModel>> ListUserProjects(string userId);
    }
}
