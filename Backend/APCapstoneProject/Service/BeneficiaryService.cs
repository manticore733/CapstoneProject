using APCapstoneProject.Model;
using APCapstoneProject.Repository;

namespace APCapstoneProject.Service
{
    public class BeneficiaryService: IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
        }

        public async Task<IEnumerable<Beneficiary>> GetAllBeneficiariesAsync()
        {
            return await _beneficiaryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByClientIdAsync(int clientUserId)
        {
            return await _beneficiaryRepository.GetByClientIdAsync(clientUserId);
        }

        public async Task<Beneficiary?> GetBeneficiaryByIdAsync(int id)
        {
            return await _beneficiaryRepository.GetByIdAsync(id);
        }

        public async Task<Beneficiary> CreateBeneficiaryAsync(Beneficiary beneficiary)
        {
            // ✅ Business Rule: Account number must be unique per client
            var existing = await _beneficiaryRepository.GetByClientIdAsync(beneficiary.ClientUserId);
            if (existing.Any(b => b.AccountNumber == beneficiary.AccountNumber))
                throw new Exception("Beneficiary with this account number already exists for this client!");

            return await _beneficiaryRepository.AddAsync(beneficiary);
        }

        public async Task UpdateBeneficiaryAsync(int id, Beneficiary updatedBeneficiary)
        {
            var existing = await _beneficiaryRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Beneficiary not found!");

            existing.BeneficiaryName = updatedBeneficiary.BeneficiaryName;
            existing.AccountNumber = updatedBeneficiary.AccountNumber;
            existing.BankName = updatedBeneficiary.BankName;
            existing.IFSC = updatedBeneficiary.IFSC;
            existing.UpdatedAt = DateTime.UtcNow;

            await _beneficiaryRepository.UpdateAsync(existing);
        }

        public async Task DeleteBeneficiaryAsync(int id)
        {
            if (!await _beneficiaryRepository.ExistsAsync(id))
                throw new KeyNotFoundException("Beneficiary not found!");

            await _beneficiaryRepository.DeleteAsync(id);
        }
    }
}
