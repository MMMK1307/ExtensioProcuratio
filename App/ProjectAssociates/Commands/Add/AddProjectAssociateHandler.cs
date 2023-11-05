using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.ProjectAssociates.Commands.Add
{
    public class AddProjectAssociateHandler : IRequestHandler<AddProjectAssociateCommand, ProjectAssociatesModel>
    {
        private readonly IProjectRepository _projectRepository;

        public AddProjectAssociateHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectAssociatesModel> Handle(AddProjectAssociateCommand request, CancellationToken cancellationToken)
        {
            var associate = new ProjectAssociatesModel(request.UserId, request.ProjectId);
            await _projectRepository.AddAssociateUser(associate);
            return associate;
        }
    }
}