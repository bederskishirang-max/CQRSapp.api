using CQRSapp.Domain.Entity;
using CQRSapp.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Application.Auth.Commands
{
    public record RegisterCommand(string Username, string Password) : IRequest<bool>;
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public RegisterCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (existingUser != null)
            {
                // User already exists, return false
                return await Task.FromResult(false);
            }

            var user = new UserEntity
            { 
             Id = Guid.NewGuid(),
                Username = request.Username, 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)

            };

            await _userRepository.AddUserAsync(user);                                       
            
            return await Task.FromResult(true);
        }
    }

}
    