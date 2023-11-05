using ExtensioProcuratio.Areas.Identity.Data;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.User.Queries.GetById
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, ApplicationUser>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApplicationUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _userRepository.GetUserById(request.UserId);
        }
    }
}