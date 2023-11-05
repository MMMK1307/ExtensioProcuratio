using MediatR;

namespace ExtensioProcuratio.App.Email.ResetPassword
{
    public record ResetPasswordEmailCommand(string Email) : IRequest<bool>;
}