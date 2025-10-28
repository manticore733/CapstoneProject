using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly BankingAppDbContext _context;

        public EmployeeRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetByClientIdAsync(int clientUserId)
        {
            return await _context.Employees
                .Where(e => e.ClientUserId == clientUserId && e.IsActive)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAndClientIdAsync(int id, int clientUserId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id &&
                                          e.ClientUserId == clientUserId &&
                                          e.IsActive);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsActive);
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            // Removed SaveChangesAsync()
        }

        public void Update(Employee employee)
        {
            employee.UpdatedAt = DateTime.UtcNow;
            _context.Employees.Update(employee);
            // Removed SaveChangesAsync()
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            employee.IsActive = false;
            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(); // SoftDelete is a complete operation
            return true;
        }

        // --- ADD THIS METHOD ---
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}