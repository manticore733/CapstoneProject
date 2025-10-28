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

        //SUPER ADMIN SECTION:

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .Include(u=>u.Role)
                .Include(u => u.Bank)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive );
        }

        public async Task AddAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }






















        //public async Task<bool> SaveChangesAsync()
        //{
        //    return (await _context.SaveChangesAsync() > 0);
        //}


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
