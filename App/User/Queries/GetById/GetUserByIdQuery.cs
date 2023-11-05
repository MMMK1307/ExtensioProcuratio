using ExtensioProcuratio.Areas.Identity.Data;
using MediatR;

namespace ExtensioProcuratio.App.User.Queries.GetById
{
    public record GetUserByIdQuery(string UserId) : IRequest<ApplicationUser>;
}