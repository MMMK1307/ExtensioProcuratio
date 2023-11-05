using ExtensioProcuratio.App.Project.Queries.GetById;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.Project.Commands.Delete
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ISender _mediator;

        public DeleteProjectHandler(ISender mediator, IProjectRepository projectRepository)
        {
            _mediator = mediator;
            _projectRepository = projectRepository;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _mediator.Send(new GetProjectByIdQuery(request.ProjectId), cancellationToken);

            if (project.UserId != request.UserId)
            {
                return false;
            }

            await _projectRepository.Delete(project);

            return true;
        }
    }
}