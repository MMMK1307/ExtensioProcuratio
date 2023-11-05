using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Roles.Commands.Create
{
    public record CreateRoleCommand(string UserId, string RoleId) : IRequest<RolesModel>;
}