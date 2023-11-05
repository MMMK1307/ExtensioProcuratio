using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.ProjectAssociates.Queries.GetByProject
{
    public class GetAssociatesByProjectHandler : IRequestHandler<GetAssociatesByProjectQuery, IEnumerable<string>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetAssociatesByProjectHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<string>> Handle(GetAssociatesByProjectQuery request, CancellationToken cancellationToken)
        {
            return await _projectRepository.ListProjectAssociates(request.ProjectId);
        }
    }
}