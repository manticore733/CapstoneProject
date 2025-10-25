using APCapstoneProject.Data;
using APCapstoneProject.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace APCapstoneProject.Repository
{
    public class BeneficiaryRepository: IBeneficiaryRepository
    {
        private readonly BankingAppDbContext _context;

        public BeneficiaryRepository(BankingAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Beneficiary>> GetAllAsync()
        {
            return await _context.Beneficiaries
                .Include(b => b.ClientUser)
                .Where(b => b.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Beneficiary>> GetByClientIdAsync(int clientUserId)
        {
            return await _context.Beneficiaries
                .Include(b => b.ClientUser)
                .Where(b => b.ClientUserId == clientUserId && b.IsActive)
                .ToListAsync();
        }

        public async Task<Beneficiary?> GetByIdAsync(int id)
        {
            return await _context.Beneficiaries
                .Include(b => b.ClientUser)
                .FirstOrDefaultAsync(b => b.BeneficiaryId == id && b.IsActive);
        }

        public async Task<Beneficiary> AddAsync(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();
            return beneficiary;
        }

        public async Task UpdateAsync(Beneficiary beneficiary)
        {
            beneficiary.UpdatedAt = DateTime.UtcNow;
            _context.Beneficiaries.Update(beneficiary);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var beneficiary = await _context.Beneficiaries.FindAsync(id);
            if (beneficiary != null)
            {
                beneficiary.IsActive = false; // Soft delete
                beneficiary.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Beneficiaries.AnyAsync(b => b.BeneficiaryId == id && b.IsActive);
        }
    }
}
