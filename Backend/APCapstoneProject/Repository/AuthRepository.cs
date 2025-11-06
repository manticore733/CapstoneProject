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
            var user = await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.UserName == username);

            // If it's a client, load  VerificationStatus relationship
            if (user is ClientUser clientUser)
            {
                await _context.Entry(clientUser)
                    .Reference(c => c.VerificationStatus)
                    .LoadAsync();
            }

            return user;
        }
    }
}
