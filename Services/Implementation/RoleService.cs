using AutoMapper;
using Contracts.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.ServiceInterfaces;


namespace Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
        }


        public async Task<RoleDto> CreateRole(RoleDto model)
        {
            var role = _mapper.Map<IdentityRole>(model.Name.ToLower());
            var createdRole = await _roleManager.CreateAsync(role);
            var roleDto = _mapper.Map<RoleDto>(createdRole);
            return roleDto;
        }


        public async Task<RoleDto> UpdateRole(string roleId, RoleDto model)
        {
            var role = await _roleManager.FindByIdAsync(roleId) ?? throw new ApplicationException("Role not found");
            role.Name = model.Name.ToLower();

            var updatedRole = await _roleManager.UpdateAsync(role);
            var roleDto = _mapper.Map<RoleDto>(updatedRole);
            return roleDto;
        }


        public async Task DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId) ?? throw new ApplicationException("Role not found");

            await _roleManager.DeleteAsync(role);
        }


        public async Task<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return roleDtos;
        }


        public async Task<RoleDto> GetRoleById(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                throw new ApplicationException("Role not found");

            var roleDto = _mapper.Map<RoleDto>(role);
            return roleDto;
        }
    }
}
