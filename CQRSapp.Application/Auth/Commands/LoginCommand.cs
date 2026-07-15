using BCrypt.Net;
using CQRSapp.Application.Auth.DTOs;
using CQRSapp.Application.Auth.Services;
using CQRSapp.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Application.Auth.Commands
{
    public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

  

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {

        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);


            if (user == null)
            {
                throw new Exception("User not found");
            }

            var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!validPassword)
            {
                throw new Exception("Invalid password");
            }

            var token = _jwtTokenGenerator.GenerateToken(user.Id,user.Username);

            return new LoginResponse
            {
                Token = token,
                Username = request.Username,
                UserId = user.Id // Replace with actual user ID from your database
            };
        }
    }

}
