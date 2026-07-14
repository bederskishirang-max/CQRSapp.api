using CQRSapp.Domain.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeesEntity>> GetAllEmployeesAsync();
        Task<EmployeesEntity> GetEmployeeByIdAsync(Guid id);
        Task<EmployeesEntity> AddEmployeeAsync(EmployeesEntity employee);
        Task<EmployeesEntity> UpdateEmployeeAsync(EmployeesEntity employee);
        Task<bool> DeleteEmployeeAsync(Guid id);
    }
}
