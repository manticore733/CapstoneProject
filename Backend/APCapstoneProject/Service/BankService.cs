using APCapstoneProject.Model;
using APCapstoneProject.Repository;

namespace APCapstoneProject.Service
{
    public class BankService: IBankService
    {
        private readonly IBankRepository _bankRepository;

        public BankService(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public async Task<IEnumerable<Bank>> GetAllBanksAsync()
        {
            return await _bankRepository.GetAllAsync();
        }

        public async Task<Bank?> GetBankByIdAsync(int id)
        {
            return await _bankRepository.GetByIdAsync(id);
        }

        public async Task<Bank> CreateBankAsync(Bank bank)
        {
            // ✅ Example business rule: IFSC must be unique
            var allBanks = await _bankRepository.GetAllAsync();
            if (allBanks.Any(b => b.IFSC == bank.IFSC))
                throw new Exception("A bank with this IFSC already exists!");

            return await _bankRepository.AddAsync(bank);
        }

        public async Task UpdateBankAsync(int id, Bank updatedBank)
        {
            var existingBank = await _bankRepository.GetByIdAsync(id);
            if (existingBank == null)
                throw new KeyNotFoundException("Bank not found!");

            // Update allowed fields
            existingBank.BankName = updatedBank.BankName;
            existingBank.IFSC = updatedBank.IFSC;
            existingBank.EstablishmentDate = updatedBank.EstablishmentDate;
            existingBank.UpdatedAt = DateTime.UtcNow;

            await _bankRepository.UpdateAsync(existingBank);
        }

        public async Task DeleteBankAsync(int id)
        {
            if (!await _bankRepository.ExistsAsync(id))
                throw new KeyNotFoundException("Bank not found!");

            await _bankRepository.DeleteAsync(id);
        }
    }
}
