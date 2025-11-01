using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly BankingAppDbContext _context;

        public AuthRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        // fetch user by username only
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);
        }
    }
}
