using CQRSapp.Domain.Employee;
using CQRSapp.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Application.Employee.Commands
{
    public record AddEmployeeCommand(EmployeesEntity Employees) : IRequest<EmployeesEntity>;

    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, EmployeesEntity>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AddEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeesEntity> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
          
           return await _employeeRepository.AddEmployeeAsync(request.Employees);
        }
    }
}
