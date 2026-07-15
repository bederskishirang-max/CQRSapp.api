using CQRSapp.Domain.Entity;
using CQRSapp.Domain.Interfaces;
using CQRSapp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Infrastructure.Respository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbcontext;

        public UserRepository(AppDbContext appDbContext)
        {
            _dbcontext = appDbContext;
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
        {
            return await _dbcontext.Users.ToListAsync();
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid id)
        {
            return await _dbcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<UserEntity> AddUserAsync(UserEntity user)
        {
            user.Id = Guid.NewGuid();
            _dbcontext.Users.Add(user);
            await _dbcontext.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity> UpdateUserAsync(UserEntity user)
        {
            var existingUser = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            existingUser.Username = user.Username;
            existingUser.PasswordHash = user.PasswordHash;
            await _dbcontext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var existingUser = await _dbcontext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (existingUser == null)
            {
                throw new Exception("User not found");
            }
            _dbcontext.Users.Remove(existingUser);
            await _dbcontext.SaveChangesAsync();
            return true;
        }

    }
}
