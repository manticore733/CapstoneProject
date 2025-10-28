using APCapstoneProject.DTO.Employee;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;

namespace APCapstoneProject.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper; // <-- ADD THIS

        public EmployeeService(IEmployeeRepository repository, IMapper mapper) // <-- ADD MAPPER
        {
            _repository = repository;
            _mapper = mapper; // <-- ADD THIS
        }

        public async Task<IEnumerable<EmployeeReadDto>> GetEmployeesByClientIdAsync(int clientUserId)
        {
            var employees = await _repository.GetByClientIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<EmployeeReadDto>>(employees);
        }

        public async Task<EmployeeReadDto?> GetEmployeeByIdAsync(int id, int clientUserId)
        {
            var employee = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (employee == null) return null;
            return _mapper.Map<EmployeeReadDto>(employee);
        }

        public async Task<EmployeeReadDto> CreateEmployeeAsync(CreateEmployeeDto employeeDto, int clientUserId)
        {
            // Business Rule: Check for duplicate email or account number for this client
            var existing = await _repository.GetByClientIdAsync(clientUserId);
            if (existing.Any(e => e.Email == employeeDto.Email))
                throw new Exception("Employee with this email already exists for this client!");

            var employee = _mapper.Map<Employee>(employeeDto);

            // Set properties not in the DTO
            employee.ClientUserId = clientUserId;
            employee.CreatedAt = DateTime.UtcNow;
            employee.IsActive = true;

            await _repository.AddAsync(employee);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EmployeeReadDto>(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto, int clientUserId)
        {
            // Get the employee *and* check ownership
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            // Map the DTO onto the existing model
            _mapper.Map(employeeDto, existing);

            _repository.Update(existing);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteEmployeeAsync(int id, int clientUserId)
        {
            // Check ownership before deleting
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            return await _repository.SoftDeleteAsync(id);
        }
    }
}