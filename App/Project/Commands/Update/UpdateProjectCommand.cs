using ExtensioProcuratio.Enumerators;
using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Project.Commands.Update
{
    public record UpdateProjectCommand(
        ProjectId Id,
        string Name,
        string Description,
        string UserId,
        string? Edital,
        string? Participants,
        bool Bolsa,
        ProjectType Type,
        SubjectAreas Subject,
        ProjectStatus Status,
        DateTime DateCreated
    ) : IRequest<ProjectModel>;
}