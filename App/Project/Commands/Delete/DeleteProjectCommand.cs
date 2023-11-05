using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Project.Commands.Delete
{
    public record DeleteProjectCommand(ProjectId ProjectId, string UserId) : IRequest<bool>;
}