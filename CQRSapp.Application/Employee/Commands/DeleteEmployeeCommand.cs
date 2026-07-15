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
    public record DeleteEmployeeCommand(Guid Employeeid) : IRequest<bool>;

    internal class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand,bool>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            
           return await _employeeRepository.DeleteEmployeeAsync(request.Employeeid);
           
        }

    }
   
}
