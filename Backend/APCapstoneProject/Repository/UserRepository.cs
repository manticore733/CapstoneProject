using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;

namespace APCapstoneProject.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly BankingAppDbContext _context;

        public UserRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive)
                .ToListAsync();
        }


        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);
        }

        public async Task AddAsync(User user)
        {
            // Only adds to the context, does NOT save.
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            // Only marks the entity for update, does NOT save.
            _context.Users.Update(user);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }


        // --- NEW METHODS ADDED BELOW ---

        public async Task<IEnumerable<User>> GetClientsByBankUserIdAsync(int bankUserId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Where(u => u.IsActive &&
                            u is ClientUser &&
                            ((ClientUser)u).BankUserId == bankUserId)
                .ToListAsync();
        }

        public async Task<ClientUser?> GetClientByBankUserIdAsync(int clientId, int bankUserId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == clientId && u.IsActive);

            // Check if it's a ClientUser AND belongs to the specified BankUser
            if (user is ClientUser clientUser && clientUser.BankUserId == bankUserId)
            {
                return clientUser;
            }

            // Not found, not a ClientUser, or doesn't belong to this bank user
            return null;
        }
















    }
}
