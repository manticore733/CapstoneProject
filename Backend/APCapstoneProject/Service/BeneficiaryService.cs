using APCapstoneProject.DTO.Beneficiary;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APCapstoneProject.Service
{
    public class BeneficiaryService : IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public BeneficiaryService(
            IBeneficiaryRepository beneficiaryRepository,
            IMapper mapper,
            IUserRepository userRepository) 
        {
            _beneficiaryRepository = beneficiaryRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<BeneficiaryReadDto>> GetBeneficiariesByClientIdAsync(int clientUserId)
        {
            
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }
            
            var beneficiaries = await _beneficiaryRepository.GetByClientIdAsync(clientUserId);
            return _mapper.Map<IEnumerable<BeneficiaryReadDto>>(beneficiaries);
        }

        public async Task<BeneficiaryReadDto?> GetBeneficiaryByIdAsync(int id, int clientUserId)
        {
            var beneficiary = await _beneficiaryRepository.GetByIdAndClientIdAsync(id, clientUserId);
            if (beneficiary == null) return null;
            return _mapper.Map<BeneficiaryReadDto>(beneficiary);
        }

        public async Task<BeneficiaryReadDto> CreateBeneficiaryAsync([FromBody] CreateBeneficiaryDto beneficiaryDto, int clientUserId)
        {
            
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }
            



            // unique account number per client
            var existing = await _beneficiaryRepository.GetByClientIdAsync(clientUserId);
            if (existing.Any(b => b.AccountNumber == beneficiaryDto.AccountNumber))
                throw new Exception("Beneficiary with this account number already exists for this client!");

            var beneficiary = _mapper.Map<Beneficiary>(beneficiaryDto);

            beneficiary.ClientUserId = clientUserId;
            beneficiary.IsActive = true;

            await _beneficiaryRepository.AddAsync(beneficiary);

            return _mapper.Map<BeneficiaryReadDto>(beneficiary);
        }

        public async Task<BeneficiaryReadDto> UpdateBeneficiaryAsync(int id,[FromBody] UpdateBeneficiaryDto beneficiaryDto, int clientUserId)
        {
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }
            
            var existing = await _beneficiaryRepository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return null; 
            }

            // use automapper
            _mapper.Map(beneficiaryDto, existing);
            await _beneficiaryRepository.UpdateAsync(existing);

            var updated = await _beneficiaryRepository.GetByIdAndClientIdAsync(existing.BeneficiaryId, clientUserId);
            return _mapper.Map<BeneficiaryReadDto>(updated);
        }

        public async Task<bool> DeleteBeneficiaryAsync(int id, int clientUserId)
        {
            var isClientUser = await _userRepository.IsActiveClientUserAsync(clientUserId);
            if (!isClientUser)
            {
                throw new KeyNotFoundException($"No active ClientUser found with ID '{clientUserId}'.");
            }

            var existing = await _beneficiaryRepository.GetByIdAndClientIdAsync(id, clientUserId);
            if (existing == null)
            {
                return false;
            }

            return await _beneficiaryRepository.DeleteAsync(id);
        }
    }
}





