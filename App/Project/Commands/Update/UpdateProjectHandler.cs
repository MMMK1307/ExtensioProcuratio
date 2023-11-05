using ExtensioProcuratio.Helper.Interfaces;
using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using Mapster;
using MediatR;

namespace ExtensioProcuratio.App.Project.Commands.Update
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, ProjectModel>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public UpdateProjectHandler(IDateTimeProvider dateTimeProvider, IProjectRepository projectRepository)
        {
            _dateTimeProvider = dateTimeProvider;
            _projectRepository = projectRepository;
        }

        public async Task<ProjectModel> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = request.Adapt<ProjectModel>();

            project.DateUpdated = _dateTimeProvider.GetBrazil();

            await _projectRepository.Update(project);

            return project;
        }
    }
}