using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.GetAll
{
    public record GetAllProjectsQuery : IRequest<IEnumerable<ProjectModel>>;
}