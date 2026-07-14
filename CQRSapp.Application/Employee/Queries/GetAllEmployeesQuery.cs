using CQRSapp.Domain.Employee;
using CQRSapp.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Application.Employee.Queries
{
    public record GetAllEmployeesQuery : IRequest<IEnumerable<EmployeesEntity>>;
    
    public class GetAllEmployeesQueriesHandler : IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeesEntity>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        public GetAllEmployeesQueriesHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<IEnumerable<EmployeesEntity>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            return await _employeeRepository.GetAllEmployeesAsync();
        }
    }
}
