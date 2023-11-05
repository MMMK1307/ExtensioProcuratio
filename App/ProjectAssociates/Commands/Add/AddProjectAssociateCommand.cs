using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.ProjectAssociates.Commands.Add
{
    public record AddProjectAssociateCommand(ProjectId ProjectId, string UserId) : IRequest<ProjectAssociatesModel>;
}