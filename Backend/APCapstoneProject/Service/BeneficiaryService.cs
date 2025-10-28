//using APCapstoneProject.Model;
//using APCapstoneProject.Repository;

//namespace APCapstoneProject.Service
//{
//    public class BeneficiaryService: IBeneficiaryService
//    {
//        private readonly IBeneficiaryRepository _beneficiaryRepository;

//        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository)
//        {
//            _beneficiaryRepository = beneficiaryRepository;
//        }

//        public async Task<IEnumerable<Beneficiary>> GetAllBeneficiariesAsync()
//        {
//            return await _beneficiaryRepository.GetAllAsync();
//        }

//        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByClientIdAsync(int clientUserId)
//        {
//            return await _beneficiaryRepository.GetByClientIdAsync(clientUserId);
//        }

//        public async Task<Beneficiary?> GetBeneficiaryByIdAsync(int id)
//        {
//            return await _beneficiaryRepository.GetByIdAsync(id);
//        }

//        public async Task<Beneficiary> CreateBeneficiaryAsync(Beneficiary beneficiary)
//        {
//            // ✅ Business Rule: Account number must be unique per client
//            var existing = await _beneficiaryRepository.GetByClientIdAsync(beneficiary.ClientUserId);
//            if (existing.Any(b => b.AccountNumber == beneficiary.AccountNumber))
//                throw new Exception("Beneficiary with this account number already exists for this client!");

//            return await _beneficiaryRepository.AddAsync(beneficiary);
//        }

//        public async Task UpdateBeneficiaryAsync(int id, Beneficiary updatedBeneficiary)
//        {
//            var existing = await _beneficiaryRepository.GetByIdAsync(id);
//            if (existing == null)
//                throw new KeyNotFoundException("Beneficiary not found!");

//            existing.BeneficiaryName = updatedBeneficiary.BeneficiaryName;
//            existing.AccountNumber = updatedBeneficiary.AccountNumber;
//            existing.BankName = updatedBeneficiary.BankName;
//            existing.IFSC = updatedBeneficiary.IFSC;
//            existing.UpdatedAt = DateTime.UtcNow;

//            await _beneficiaryRepository.UpdateAsync(existing);
//        }

//        public async Task DeleteBeneficiaryAsync(int id)
//        {
//            if (!await _beneficiaryRepository.ExistsAsync(id))
//                throw new KeyNotFoundException("Beneficiary not found!");

//            await _beneficiaryRepository.DeleteAsync(id);
//        }
//    }
//}










using APCapstoneProject.DTO.Beneficiary;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;

namespace APCapstoneProject.Service
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IMapper _mapper; // <-- ADD THIS

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository, IMapper mapper) // <-- ADD MAPPER
        {
            _beneficiaryRepository = beneficiaryRepository;
            _mapper = mapper; // <-- ADD THIS
        }

        public async Task<IEnumerable<BeneficiaryReadDto>> GetBeneficiariesByClientIdAsync(int clientUserId)
        {
            var beneficiaries = await _beneficiaryRepository.GetByClientIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<BeneficiaryReadDto>>(beneficiaries);
        }

        public async Task<BeneficiaryReadDto?> GetBeneficiaryByIdAsync(int id, int clientUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAndClientIdAsync(id, clientUserId);
            if (beneficiary == null) return null;
            return _mapper.Map<BeneficiaryReadDto>(beneficiary);
        }

        public async Task<BeneficiaryReadDto> CreateBeneficiaryAsync(CreateBeneficiaryDto beneficiaryDto, int clientUserId)
        {
            // Business Rule: Account number must be unique per client
            var existing = await _beneficiaryRepository.GetByClientIdAsync(clientUserId);
            if (existing.Any(b => b.AccountNumber == beneficiaryDto.AccountNumber))
                throw new Exception("Beneficiary with this account number already exists for this client!");

            var beneficiary = _mapper.Map<Beneficiary>(beneficiaryDto);

            // Set properties not in the DTO
            beneficiary.ClientUserId = clientUserId;
            beneficiary.CreatedAt = DateTime.UtcNow;
            beneficiary.IsActive = true;

            await _beneficiaryRepository.AddAsync(beneficiary);
            await _beneficiaryRepository.SaveChangesAsync();

            return _mapper.Map<BeneficiaryReadDto>(beneficiary);
        }

        public async Task<bool> UpdateBeneficiaryAsync(int id, UpdateBeneficiaryDto beneficiaryDto, int clientUserId)
        {
            // Get the beneficiary *and* check ownership
            var existing = await _beneficiaryRepository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            // use automapper
            existing.BeneficiaryName = updatedBeneficiary.BeneficiaryName;
            existing.AccountNumber = updatedBeneficiary.AccountNumber;
            existing.BankName = updatedBeneficiary.BankName;
            existing.IFSC = updatedBeneficiary.IFSC;

            _beneficiaryRepository.Update(existing);
            return await _beneficiaryRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteBeneficiaryAsync(int id, int clientUserId)
        {
            // Check ownership before deleting
            var existing = await _beneficiaryRepository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false; // Not found or doesn't belong to this user
            }

            return await _beneficiaryRepository.SoftDeleteAsync(id);
        }
    }
}





