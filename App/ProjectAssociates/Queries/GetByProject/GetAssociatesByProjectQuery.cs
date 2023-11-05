using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.ProjectAssociates.Queries.GetByProject
{
    public record GetAssociatesByProjectQuery(ProjectId ProjectId) : IRequest<IEnumerable<string>>;
}