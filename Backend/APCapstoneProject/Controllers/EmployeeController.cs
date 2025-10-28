using APCapstoneProject.DTO.Employee;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Route("api/employees")] // Changed route to plural
    [ApiController]
    public class EmployeesController : ControllerBase // Renamed controller
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpGet("myemployees/{clientUserId}")]
        public async Task<IActionResult> GetMyEmployees(int clientUserId)
        {
            var employees = await _service.GetEmployeesByClientIdAsync(clientUserId);
            return Ok(employees);
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpGet("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> GetMyEmployee(int id, int clientUserId)
        {
            var employee = await _service.GetEmployeeByIdAsync(id, clientUserId);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpPost("createdby/{clientUserId}")]
        public async Task<IActionResult> Create(int clientUserId, [FromBody] CreateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateEmployeeAsync(employeeDto, clientUserId);

            return CreatedAtAction(nameof(GetMyEmployee), new { id = created.EmployeeId, clientUserId = clientUserId }, created);
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpPut("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> Update(int id, int clientUserId, [FromBody] UpdateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _service.UpdateEmployeeAsync(id, employeeDto, clientUserId);

            if (!success) return NotFound("Employee not found or you do not have permission.");
            return NoContent();
        }

        // [Authorize(Roles = "CLIENT_USER")] // <-- Add this later
        [HttpDelete("{id}/ownedby/{clientUserId}")]
        public async Task<IActionResult> Delete(int id, int clientUserId)
        {
            var success = await _service.DeleteEmployeeAsync(id, clientUserId);

            if (!success) return NotFound("Employee not found or you do not have permission.");
            return NoContent();
        }
    }
}