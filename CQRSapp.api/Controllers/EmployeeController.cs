using CQRSapp.Application.Employee.Commands;
using CQRSapp.Application.Employee.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRSapp.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly ISender _sender;


        public EmployeeController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);


        }



        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        //[HttpPut("{employeeId:guid}")]
        //public async Task<IActionResult> UpdateEmployee(
        // [FromRoute] Guid employeeId,
        // [FromBody] UpdateEmployeeCommand command) // Use a Request DTO here
        //{
        //    // Map the route ID and body request to your internal Command
            

        //    var result = await _sender.Send(command);
        //    return Ok(result); // Or 'Return NoContent();' (HTTP 204) if you don't return data
        //}

        [HttpGet("DisplayAllEmployee")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _sender.Send(new GetAllEmployeesQuery());
            return Ok(result);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetEmployeeById([FromRoute] Guid employeeId)
        //{
        //    var result = await _sender.Send(new GetEmployeesByIDQuery(employeeId));
        //    return Ok(result);
        //}

        [HttpGet("{employeeId:guid}")] // Explicitly constrain the route to a GUID
        public async Task<IActionResult> GetEmployeeById([FromRoute] Guid employeeId)
        {
            var result = await _sender.Send(new GetEmployeesByIDQuery(employeeId));

            if (result == null)
            {
                return NotFound(); // Return 404 if the employee doesn't exist
            }

            return Ok(result);
        }

        [HttpDelete("{employeeId:guid}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid employeeId)
        {
            var result = await _sender.Send(new DeleteEmployeeCommand(employeeId));
            return Ok(result);
        }
    }
}
