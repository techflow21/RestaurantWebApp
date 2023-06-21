using AutoMapper;
using Contracts.DTOs;
using Contracts.Logger;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Presentation;
using Services.JwtFeatures;
using Services.ServiceInterfaces;
using System.IdentityModel.Tokens.Jwt;



namespace Services.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IMailKitEmailService _emailService;
        private readonly IAwsStorageService _awsStorageService;
        private readonly ILocalStorageService _localStorageService;
        private readonly JwtHandler _jwtHandler;

        public AuthenticationService(ILoggerManager logger, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailKitEmailService emailService, IAwsStorageService awsStorageService, ILocalStorageService localStorageService, IMapper mapper, JwtHandler jwtHandler)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _jwtHandler = jwtHandler;
            _emailService = emailService;
            _awsStorageService = awsStorageService;
            _localStorageService = localStorageService;
        }


        public async Task<RegisterDto> Register(RegisterDto model)
        {
            var user = _mapper.Map<User>(model);
            user.UserName = model.UserName.ToLower();
            user.IsActive = true;
            user.IsAuthenticated = false;
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
                //user.EmailConfirmed = true;
                //await _userManager.UpdateAsync(user);
                await _userManager.CreateAsync(user, model.Password);

                var userRoleExist = await _roleManager.RoleExistsAsync("user");

                if (!userRoleExist)
                {
                    var newUserRole = new IdentityRole { Name = "user" };
                    await _roleManager.CreateAsync(newUserRole);
                }
                await _userManager.AddToRoleAsync(user, "user");

                var emailConfirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await SendConfirmationEmail(user, emailConfirmToken);

                return model;
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new ApplicationException("Failed to register user");
            }

            var userRoleExists = await _roleManager.RoleExistsAsync("user");

            if (!userRoleExists)
            {
                var newUserRole = new IdentityRole { Name = "user" };
                await _roleManager.CreateAsync(newUserRole);
            }
            await _userManager.AddToRoleAsync(user, "user");

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendConfirmationEmail(user, emailConfirmationToken);

            return model;
        }


        public async Task<AuthenticationResponseDto> Authenticate(AuthenticationRequestDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName.ToLower());
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthenticationResponseDto { ErrorMessage = "Invalid Authentication" };

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new ApplicationException("Your account has not been confirmed, check your email");
            }
            if (!user.IsActive)
            {
                throw new ApplicationException("Your account has been deactivated");
            }
            /*var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                throw new ApplicationException("Invalid credentials");*/

            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new AuthenticationResponseDto { IsAuthSuccessful = true, AccessToken = token };
        }


        //This service method should only at development stage
        public async Task ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                throw new ApplicationException("Invalid email confirmation link.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                throw new ApplicationException("Failed to confirm email.");
            }
        }


        public async Task<AuthenticationResponseDto> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                throw new ApplicationException("Invalid email or email not confirmed");

            var forgotPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await SendForgotPasswordEmail(user, forgotPasswordToken);

            var token = GenerateJwtToken(user.Email);
            return await token;
        }


        public async Task<AuthenticationResponseDto> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                throw new ApplicationException("Invalid email or email not confirmed");

            if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmNewPassword))
                throw new ApplicationException("New password and confirm new password are required");

            if (model.NewPassword != model.ConfirmNewPassword)
                throw new ApplicationException("New password and confirm new password do not match");

            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!isOldPasswordValid)
                throw new ApplicationException("Invalid old password");

            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, model.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.SecurityStamp = Guid.NewGuid().ToString();
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new ApplicationException("Failed to update password");

            /*var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var authResponse = new AuthenticationResponseDto
            {
                AccessToken = token,
            };*/
            var token = GenerateJwtToken(user.Email);
            return await token;
        }


        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }


        private async Task<AuthenticationResponseDto> GenerateJwtToken(string username)
        {
            var user = await _userManager.FindByNameAsync(username.ToLower());

            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var authResponse = new AuthenticationResponseDto
            {
                AccessToken = token,
            };

            return authResponse;
        }


        /*private AuthenticationResponseDto GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var accessTokenExpirationMinutes = Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"]);
            //var refreshTokenExpirationDays = Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
        }),
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var refreshToken = GenerateRefreshToken();
            tokenDescriptor.Subject.AddClaim(new Claim("refreshToken", refreshToken));

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResponseDto
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                AccessTokenExpiration = tokenDescriptor.Expires.Value,
                IsEmailConfirmed = true
            };
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var refreshTokenExpirationDays = Convert.ToInt32(_configuration["Jwt:RefreshTokenExpirationDays"]);
            var expirationDate = DateTime.UtcNow.AddDays(refreshTokenExpirationDays);

            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpirationDate = expirationDate
            };

            // Save the refresh token to the database or another secure storage

            return refreshToken.Token;
        }*/


        private async Task SendConfirmationEmail(User user, string emailConfirmationToken)
        {
            var confirmationLink = $"https://localhost.com:7184/api/authentication/confirm?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SOB-Authentication-System", "noreply@sobtech.com"));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = "Account Confirmation";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Hello {user.UserName},</p><p>Please click the following link to confirm your account: <a href=\"{confirmationLink}\">Confirm Email</a></p>";

            message.Body = bodyBuilder.ToMessageBody();

            await _emailService.SendEmailAsync(message);
        }


        private async Task SendForgotPasswordEmail(User user, string forgotPasswordToken)
        {
            var forgotPasswordLink = $"https://localhost.com:7184/api/authentication/confirm?userId={user.Id}&token={Uri.EscapeDataString(forgotPasswordToken)}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SOB-Authentication-System", "noreply@sobtech.com"));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = "Forgot Password Link";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $"<p>Hello {user.UserName},</p><p>Please click the following link to reset your account password: <a href=\"{forgotPasswordLink}\">Reset Password</a></p>";

            message.Body = bodyBuilder.ToMessageBody();

            await _emailService.SendEmailAsync(message);
        }
    }
}
