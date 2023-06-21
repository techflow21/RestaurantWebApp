using Contracts.DTOs;

namespace Services.ServiceInterfaces
{
    public interface IRoleService
    {
        Task<RoleDto> CreateRole(RoleDto model);
        Task<RoleDto> UpdateRole(string roleId, RoleDto model);
        Task DeleteRole(string roleId);
        Task<IEnumerable<RoleDto>> GetRoles();
        Task<RoleDto> GetRoleById(string roleId);
    }
}
