using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.GetByUser
{
    public record GetProjectByUserQuery(string UserId) : IRequest<IEnumerable<ProjectModel>>;
}