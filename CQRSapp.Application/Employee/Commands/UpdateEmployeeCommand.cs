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
    public record UpdateEmployeeCommand(Guid EmployeeID, EmployeesEntity Employees) : IRequest<EmployeesEntity>;
    

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeesEntity>
    {
        private readonly IEmployeeRepository _employeeRepository;
        public UpdateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<EmployeesEntity> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            return await _employeeRepository.UpdateEmployeeAsync(request.Employees);
        }
}
}
