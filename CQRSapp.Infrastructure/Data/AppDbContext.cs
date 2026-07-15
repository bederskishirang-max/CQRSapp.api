
using CQRSapp.Domain.Employee;
using CQRSapp.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Infrastructure.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<EmployeesEntity> Employees { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}
