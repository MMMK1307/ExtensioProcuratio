using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.CountByUser
{
    public class CountProjectByUserHandler : IRequestHandler<CountProjectByUserQuery, int>
    {
        private readonly IProjectRepository _projectRepository;

        public CountProjectByUserHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<int> Handle(CountProjectByUserQuery request, CancellationToken cancellationToken)
        {
            return await _projectRepository.CountUserProjects(request.UserId);
        }
    }
}