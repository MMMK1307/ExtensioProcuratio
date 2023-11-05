using MediatR;

namespace ExtensioProcuratio.App.Email.EmailConfirmationByAdmin
{
    public record EmailConfirmationByAdminCommand(string UserId, string Name) : IRequest<(bool Result, string Message)>;
}