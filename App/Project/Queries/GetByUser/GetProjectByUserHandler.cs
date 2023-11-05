using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.GetByUser
{
    public class GetProjectByUserHandler : IRequestHandler<GetProjectByUserQuery, IEnumerable<ProjectModel>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectByUserHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectModel>> Handle(GetProjectByUserQuery request, CancellationToken cancellationToken)
        {
            return await _projectRepository.ListUserProjects(request.UserId);
        }
    }
}