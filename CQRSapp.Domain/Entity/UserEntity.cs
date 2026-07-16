using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Domain.Entity
{
    public class UserEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string DateCreated { get; set; } = DateTime.Now.ToString();
    }
}
