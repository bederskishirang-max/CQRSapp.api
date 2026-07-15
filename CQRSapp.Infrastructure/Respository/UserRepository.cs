using CQRSapp.Domain.Interfaces;
using CQRSapp.Infrastructure.Data;
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
    }
}
