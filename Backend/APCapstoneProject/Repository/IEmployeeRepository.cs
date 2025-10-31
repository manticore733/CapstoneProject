using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IEmployeeRepository
    {
        // Changed: Gets only employees for a specific client
        Task<IEnumerable<Employee>> GetByClientIdAsync(int clientUserId);

        // New: Gets a single employee *only* if it belongs to the client
        Task<Employee?> GetByIdAndClientIdAsync(int id, int clientUserId);

        Task<Employee?> GetByIdAsync(int id); // Keep this for internal use

        // Changed: Consistent with our new pattern
        Task AddAsync(Employee employee);
        Task Update(Employee employee); // Changed from Task<Employee>
        Task<bool> DeleteAsync(int id);
        
    }
}