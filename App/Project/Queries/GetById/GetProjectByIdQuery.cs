using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.GetById
{
    public record GetProjectByIdQuery(ProjectId ProjectId) : IRequest<ProjectModel>;
}