using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Mapster;
using MediatR;

namespace ExtensioProcuratio.App.Project.Commands.Create
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ProjectModel>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateProjectHandler(IProjectRepository projectRepository, IDateTimeProvider dateTimeProvider)
        {
            _projectRepository = projectRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ProjectModel> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            ProjectModel project = request.Adapt<ProjectModel>();

            project.Id = new ProjectId(Guid.NewGuid().ToString());
            project.DateCreated = _dateTimeProvider.GetBrazil();

            await _projectRepository.Create(project);
            await _projectRepository.AddAssociateUser(
                new ProjectAssociatesModel(project.UserId, project.Id));

            return project;
        }
    }
}