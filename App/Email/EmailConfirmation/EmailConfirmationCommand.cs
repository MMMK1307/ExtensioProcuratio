using MediatR;

namespace ExtensioProcuratio.App.Email.EmailConfirmation
{
    public record EmailConfirmationCommand(string Email, string ReturnUrl) : IRequest<(bool Result, string Message)>;
}