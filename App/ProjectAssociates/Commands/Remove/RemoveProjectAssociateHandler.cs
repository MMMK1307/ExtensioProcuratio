using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.ProjectAssociates.Commands.Remove
{
    public class RemoveProjectAssociateHandler : IRequestHandler<RemoveProjectAssociateCommand, ProjectAssociatesModel>
    {
        private readonly IProjectRepository _projectRepository;

        public RemoveProjectAssociateHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectAssociatesModel> Handle(RemoveProjectAssociateCommand request, CancellationToken cancellationToken)
        {
            var associate = new ProjectAssociatesModel(request.UserId, request.ProjectId);
            await _projectRepository.RemoveAssociateUsers(associate);
            return associate;
        }
    }
}