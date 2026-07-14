using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRSapp.Application.Employee.Queries
{
    public record GetEmployeesByIDQueries : IRequest<bool>;

    public class GetEmployeesByIDQueriesHandler : IRequestHandler<GetEmployeesByIDQueries, bool>
    {
        public Task<bool> Handle(GetEmployeesByIDQueries request, CancellationToken cancellationToken)
        {
            // Implement your logic to get employees by ID here
            throw new NotImplementedException();
        }
    }


}
