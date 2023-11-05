using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.GetById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectModel>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectByIdHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectModel> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            return await _projectRepository.ListProjectById(request.ProjectId);
        }
    }
}