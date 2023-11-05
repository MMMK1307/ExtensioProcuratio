using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Email.AdoptionRequest
{
    public record AdoptionRequestCommand(ProjectId ProjectId, string UserId) : IRequest<(bool Result, string Message)>;
}