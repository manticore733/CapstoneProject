using APCapstoneProject.Model;

namespace APCapstoneProject.Service
{
   
        public interface IEmployeeService
        {
            Task<IEnumerable<Employee>> GetAllAsync();
            Task<Employee?> GetByIdAsync(int id);
            Task<Employee> CreateAsync(Employee employee);
            Task<Employee?> UpdateAsync(int id, Employee employee);
            Task<bool> SoftDeleteAsync(int id);
        }
   
}
