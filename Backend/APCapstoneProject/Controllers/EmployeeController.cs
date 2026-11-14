using APCapstoneProject.DTO.Employee;
using APCapstoneProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Controllers
{
    [Authorize(Roles = "CLIENT_USER")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service)
        {
            _service = service;
        }

        [HttpGet("myemployees")]
        public async Task<IActionResult> GetMyEmployees()
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var employees = await _service.GetEmployeesByClientIdAsync(clientUserId);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyEmployee(int id)
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var employee = await _service.GetEmployeeByIdAsync(id, clientUserId);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var created = await _service.CreateEmployeeAsync(employeeDto, clientUserId);

            return CreatedAtAction(nameof(GetMyEmployee), new { id = created.EmployeeId, clientUserId = clientUserId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto employeeDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var success = await _service.UpdateEmployeeAsync(id, employeeDto, clientUserId);

            if (!success) return NotFound("Employee not found or you do not have permission.");
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);
            var success = await _service.DeleteEmployeeAsync(id, clientUserId);

            if (!success) return NotFound("Employee not found or you do not have permission.");
            return NoContent();
        }


        [HttpPost("uploadExcel")]
        public async Task<IActionResult> UploadEmployeeExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var clientUserId = int.Parse(User.FindFirst("UserId")!.Value);

            var result = await _service.ProcessEmployeeExcelAsync(file, clientUserId);

            return Ok(result);
        }



    }
}