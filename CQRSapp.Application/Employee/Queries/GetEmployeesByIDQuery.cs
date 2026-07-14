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
    public record GetEmployeesByIDQuery(Guid EmployeeID) : IRequest<EmployeesEntity>;

    public class GetEmployeesByIDQueriesHandler : IRequestHandler<GetEmployeesByIDQuery, EmployeesEntity>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeesByIDQueriesHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeesEntity> Handle(GetEmployeesByIDQuery request, CancellationToken cancellationToken)
        {
            // Implement your logic to get employees by ID here
            return await _employeeRepository.GetEmployeeByIdAsync(request.EmployeeID);
        }
    }


}
