using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRSapp.Domain.Employee;
using CQRSapp.Domain.Interfaces;
using CQRSapp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CQRSapp.Infrastructure.Respository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _dbcontext;

        public EmployeeRepository(AppDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public async Task<IEnumerable<EmployeesEntity>> GetAllEmployeesAsync()
        {
            return await _dbcontext.Employees.ToListAsync();
        }


        public async Task<EmployeesEntity> GetEmployeeByIdAsync(Guid id)
        {
            return await _dbcontext.Employees.FirstOrDefaultAsync(x=>x.Id == id);
        }


        public async Task<EmployeesEntity> AddEmployeeAsync(EmployeesEntity employee)
        {
            employee.Id = Guid.NewGuid();
            _dbcontext.Employees.Add(employee);
            await _dbcontext.SaveChangesAsync();
            return employee;
        }

        public async Task<EmployeesEntity> UpdateEmployeeAsync(EmployeesEntity employee)
        {
            var existingEmployee = await _dbcontext.Employees.FirstOrDefaultAsync(x => x.Id == employee.Id);
            if (existingEmployee == null)
            {
                throw new Exception("Employee not found");
            }
            existingEmployee.Name = employee.Name;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            await _dbcontext.SaveChangesAsync();
            return existingEmployee;
        }


        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            var existingEmployee = await _dbcontext.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (existingEmployee == null)
            {
                throw new Exception("Employee not found");
            }
            _dbcontext.Employees.Remove(existingEmployee);
            await _dbcontext.SaveChangesAsync();
            return true;
        }


    }
}
