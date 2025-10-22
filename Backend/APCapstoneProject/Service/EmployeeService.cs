using APCapstoneProject.Model;
using APCapstoneProject.Repository;

namespace APCapstoneProject.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            employee.CreatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(employee);
        }

        public async Task<Employee?> UpdateAsync(int id, Employee employee)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.EmployeeName = employee.EmployeeName;
            existing.Email = employee.Email;
            existing.AccountNumber = employee.AccountNumber;
            existing.BankName = employee.BankName;
            existing.IFSC = employee.IFSC;
            existing.Salary = employee.Salary;
            existing.DateOfJoining = employee.DateOfJoining;
            existing.DateOfLeaving = employee.DateOfLeaving;
            existing.ClientUserId = employee.ClientUserId;

            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }
    }
}
