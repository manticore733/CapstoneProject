using APCapstoneProject.DTO.Employee;

namespace APCapstoneProject.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeReadDto>> GetEmployeesByClientIdAsync(int clientUserId);
        Task<EmployeeReadDto?> GetEmployeeByIdAsync(int id, int clientUserId);
        Task<EmployeeReadDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto, int clientUserId);
        Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto, int clientUserId);
        Task<bool> DeleteEmployeeAsync(int id, int clientUserId);
    }
}