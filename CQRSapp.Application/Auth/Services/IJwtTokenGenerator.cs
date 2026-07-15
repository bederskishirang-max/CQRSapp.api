using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Application.Auth.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string username);
    }
}
