using ExtensioProcuratio.Models;
using ExtensioProcuratio.Repositories.Interface;
using MediatR;

namespace ExtensioProcuratio.App.Roles.Commands.Create
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, RolesModel>
    {
        private readonly IUserRepository _userRepository;

        public CreateRoleHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<RolesModel> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var newRole = new RolesModel() { RoleId = request.RoleId, UserId = request.UserId };
            await _userRepository.CreateRole(newRole);
            return newRole;
        }
    }
}