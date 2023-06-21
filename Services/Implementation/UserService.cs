
using AutoMapper;
using Contracts.DTOs;
using Domain.Entities;
using Domain.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Presentation;
using Services.ServiceInterfaces;

namespace Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailKitEmailService _emailService;
        private readonly IAwsStorageService _awsStorageService;
        private readonly ILocalStorageService _localStorageService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IAwsStorageService awsStorageService, ILocalStorageService localStorageService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IMailKitEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRepository = _unitOfWork.GetRepository<User>();
            _awsStorageService = awsStorageService;
            _localStorageService = localStorageService;
            _roleManager = roleManager;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<RegisterDto> AddModeratorUser(RegisterDto model)
        {
            var user = _mapper.Map<User>(model);
            user.UserName = model.UserName.ToLower();
            user.IsActive = true;
            user.CreatedDate = DateTime.Now;
            

            var userExists = await _userManager.FindByNameAsync(user.UserName);

            if (userExists != null)
            {
                throw new ApplicationException("User already exists!");
            }

            if (model.Image != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                await _awsStorageService.SaveImageToAWSStorage(model.Image, fileName);
                var imageUrl = await _localStorageService.SaveImageToLocalFileSystem(model.Image, fileName);

                user.ImageUrl = imageUrl;
                user.EmailConfirmed = true;
                //await _userManager.UpdateAsync(user);
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException("Failed to register user");
            }

            var userRoleExists = await _roleManager.RoleExistsAsync("moderator");

            if (!userRoleExists)
            {
                var newUserRole = new IdentityRole { Name = "moderator" };
                await _roleManager.CreateAsync(newUserRole);
            }
            await _userManager.AddToRoleAsync(user, "moderator");

            //var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //await SendConfirmationEmail(user, emailConfirmationToken);

            return model;
        }


        public async Task<RegisterDto> AddAdminUser(RegisterDto model)
        {
            var user = new User
            {
                UserName = model.UserName.ToLower(),
                LastName = model.LastName,
                FirstName = model.FirstName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                State = model.State,
                Address = model.Address,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            var userExists = await _userManager.FindByNameAsync(user.UserName);

            if (userExists != null)
            {
                throw new ApplicationException("User already exists!");
            }

            if (model.Image != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                await _awsStorageService.SaveImageToAWSStorage(model.Image, fileName);
                var imageUrl = await _localStorageService.SaveImageToLocalFileSystem(model.Image, fileName);

                user.ImageUrl = imageUrl;
                user.EmailConfirmed = true;
                //await _userManager.UpdateAsync(user);
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException("Failed to register user");
            }

            var userRoleExists = await _roleManager.RoleExistsAsync("admin");
            if (!userRoleExists)
            {
                var newUserRole = new IdentityRole { Name = "admin" };
                await _roleManager.CreateAsync(newUserRole);
            }
            await _userManager.AddToRoleAsync(user, "admin");

            //var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //await SendConfirmationEmail(user, emailConfirmationToken);

            return model;
        }


        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return userDtos;
        }


        public async Task<RegisterDto> UpdateUser(string id, RegisterDto model)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            _mapper.Map(model, user);

            if (model.Image != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);

                await _awsStorageService.SaveImageToAWSStorage(model.Image, fileName);
                var imageUrl = await _localStorageService.SaveImageToLocalFileSystem(model.Image, fileName);
                user.ImageUrl = imageUrl;
                await _userManager.UpdateAsync(user);
            }
            await _unitOfWork.SaveChangesAsync();
            var updatedUserDto = _mapper.Map<RegisterDto>(user);
            return updatedUserDto;
        }


        public async Task<bool> DeleteUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }
            await _userRepository.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeactivateUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }
            user.IsActive = false;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<UserDto>> SearchUsers(string searchQuery)
        {
            var users = await _userRepository.GetByAsync(user =>
                user.UserName.Contains(searchQuery) ||
                user.FirstName.Contains(searchQuery) ||
                user.LastName.Contains(searchQuery)
            );

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return userDtos;
        }


        public async Task AddUserToRole(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username.ToLower());

            if (user == null)
            {
                throw new ApplicationException("User not found");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName.ToLower());

            if (!roleExists)
            {
                throw new ApplicationException("Role not found");
            }
            await _userManager.AddToRoleAsync(user, roleName);
            await _unitOfWork.SaveChangesAsync();
        }


        public Task<long> GetTotalRegisteredUsers()
        {
            var count = _userRepository.CountAsync();
            return count;
        }
    }
}
