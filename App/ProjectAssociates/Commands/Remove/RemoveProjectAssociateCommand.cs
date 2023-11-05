using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.ProjectAssociates.Commands.Remove
{
    public record RemoveProjectAssociateCommand(ProjectId ProjectId, string UserId) : IRequest<ProjectAssociatesModel>;
}