using Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ServiceInterfaces;

namespace RestaurantWebApp.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("add-admin")]
        public async Task<ActionResult<RegisterDto>> CreateAdminUser(RegisterDto model)
        {
            var createdUser = await _userService.AddAdminUser(model);
            return Ok(createdUser);
        }


        [HttpPost("add-moderator")]
        public async Task<ActionResult<RegisterDto>> CreateModeratorUser(RegisterDto model)
        {
            var createdUser = await _userService.AddModeratorUser(model);
            return Ok(createdUser);
        }


        [HttpGet("get-users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }


        [HttpPut("update-user/{id}")]
        public async Task<ActionResult<RegisterDto>> UpdateUser(string id, RegisterDto model)
        {
            var updatedUser = await _userService.UpdateUser(id, model);
            return Ok(updatedUser);
        }


        [HttpDelete("delete-user/{id}")]
        public async Task<ActionResult<bool>> DeleteUser(string id)
        {
            var result = await _userService.DeleteUser(id);
            return Ok(result);
        }


        [HttpPut("deactivate-user/{id}")]
        public async Task<ActionResult<bool>> DeactivateUser(string id)
        {
            var result = await _userService.DeactivateUser(id);
            return Ok(result);
        }


        [HttpGet("search-users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string query)
        {
            var users = await _userService.SearchUsers(query);
            return Ok(users);
        }


        [HttpPost("add-user-to-role")]
        public async Task<ActionResult> AddUserToRole([FromBody] AddUserToRoleDto model)
        {
            await _userService.AddUserToRole(model.UserName, model.RoleName);
            return Ok();
        }


        [HttpGet("total-registered-users")]
        public async Task<ActionResult<long>> GetTotalRegisteredUsers()
        {
            var count = await _userService.GetTotalRegisteredUsers();
            return Ok(count);
        }
    }
}
