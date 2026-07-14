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
    public record GetEmployeesByIDQueries(Guid EmployeeID) : IRequest<EmployeesEntity>;

    public class GetEmployeesByIDQueriesHandler : IRequestHandler<GetEmployeesByIDQueries, EmployeesEntity>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeesByIDQueriesHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeesEntity> Handle(GetEmployeesByIDQueries request, CancellationToken cancellationToken)
        {
            // Implement your logic to get employees by ID here
            return await _employeeRepository.GetEmployeeByIdAsync(request.EmployeeID);
        }
    }


}
