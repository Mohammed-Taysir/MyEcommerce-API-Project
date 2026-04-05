using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationService(
            UserManager<ApplicationUser> userManager, 
            IEmailSender emailSender, 
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor

            )

        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }



        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return new RegisterResponse()
                {
                    Success = false,
                    Message = "Error",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            await _userManager.AddToRoleAsync(user, "User");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);

            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/account/ConfirmEmail?token={token}&userId={user.Id}";

            await _emailSender.SendEmailAsync(user.Email, "welcome", $"<h1>Welcome {request.UserName}</h1>" + $"<a href = '{emailUrl}'>confirm</a>");

            return new RegisterResponse() { Success = true, Message = "Success" };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new LoginResponse() { Success = false, Message = "Invalid Email" };

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new LoginResponse() {Success = false, Message = "Email Is Not Confirmed" };

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
                return new LoginResponse() { Success = false, Message = "Invalid Password" };
            return new LoginResponse() { Success = true, Message = "Success", AccessToken = await GenerateAccessToken(user)};
        }

        private async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 issuer: _configuration["Jwt:Issuer"],
                 audience: _configuration["Jwt:Audience"],
                 claims: userClaims,
                 expires: DateTime.Now.AddDays(5),
                 signingCredentials: credentials
         );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return false;

            return true;
        }

        public async Task<ForgetPasswordResponse> RequestPasswordResetAsync(ForgetPasswordRequest request) {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new ForgetPasswordResponse()
                {
                    Success = false,
                    Message = "Email Is Not Found"
                };

            var random = new Random();
            var code = random.Next(1000, 9999).ToString();

            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await _userManager.UpdateAsync(user);

            await _emailSender.SendEmailAsync(request.Email, "Reset Password", $"<p>Code Is: {code}</p>");

            return new ForgetPasswordResponse()
            {
                Success = true,
                Message = "Code Sent To Your Email"
            };

        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Email Is Not Found"
                };

            if(user.CodeResetPassword != request.Code)
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Invalid Code"
                };


            if(user.PasswordResetCodeExpiry < DateTime.UtcNow)
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Code Expired"
                };

            var isSamed = await _userManager.CheckPasswordAsync(user, request.NewPassword);
            if(isSamed)
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "New Password Must Be Different"
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if(!result.Succeeded)
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Reset Password Failed"
                };

            await _emailSender.SendEmailAsync(request.Email, "Change Password", "<p>Password Changed Succesfully</p>");

            return new ResetPasswordResponse() { 
                Success = true, 
                Message = "Password Changed Successfully" 
            };

        }
    }
}
