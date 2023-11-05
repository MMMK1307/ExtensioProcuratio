using ExtensioProcuratio.Areas.Identity.Data;
using MediatR;

namespace ExtensioProcuratio.App.User.Commands.Update
{
    public record UpdateUserCommand(ApplicationUser User) : IRequest<ApplicationUser>;
}