using MediatR;

namespace ExtensioProcuratio.App.Project.Queries.CountByUser
{
    public record CountProjectByUserQuery(string UserId) : IRequest<int>;
}