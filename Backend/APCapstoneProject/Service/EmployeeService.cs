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
        private readonly IUserRepository _userRepository;

        public EmployeeService(
             IEmployeeRepository repository,
             IUserRepository userRepository, // <-- 2. INJECT IT HERE
             IMapper mapper)
        {
            _repository = repository;
            _userRepository = userRepository; // <-- 3. ASSIGN IT
            _mapper = mapper;
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
            // --- 4. USE YOUR NEW REPOSITORY METHOD ---
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }
            // --- END OF VALIDATION ---

            // Business Rule: Check for duplicate email...
            var existing = await _repository.GetByClientIdAsync(clientUserId);
            

            var employee = _mapper.Map<Employee>(employeeDto);

            employee.ClientUserId = clientUserId;
            employee.IsActive = true;

            await _repository.AddAsync(employee);
            return _mapper.Map<EmployeeReadDto>(employee);
        }




        public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeDto employeeDto, int clientUserId)
        {
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }

            // Get the employee *and* check ownership
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            // Map the DTO onto the existing model
            _mapper.Map(employeeDto, existing);

            await _repository.Update(existing);
            return true;

        }

        public async Task<bool> DeleteEmployeeAsync(int id, int clientUserId)
        {
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }

            // Check ownership before deleting
            var existing = await _repository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            return await _repository.DeleteAsync(id);
        }
    }
}