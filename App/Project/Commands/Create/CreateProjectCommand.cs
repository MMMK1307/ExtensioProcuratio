using ExtensioProcuratio.Enumerators;
using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Project.Commands.Create
{
    public record CreateProjectCommand(
        string Name,
        string Description,
        string UserId,
        string? Edital,
        string? Participants,
        bool Bolsa,
        ProjectType Type,
        SubjectAreas Subject,
        ProjectStatus Status
    ) : IRequest<ProjectModel>;
}