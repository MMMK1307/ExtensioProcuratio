using ExtensioProcuratio.Models;
using MediatR;

namespace ExtensioProcuratio.App.Email.AdoptionFeedback
{
    public record AdoptionFeedbackCommand(ProjectId ProjectId, string UserId, string Feedback) : IRequest<(bool Result, string Message)>;
}