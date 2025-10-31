﻿using APCapstoneProject.DTO.User.ClientUser;
using APCapstoneProject.Model;
using APCapstoneProject.Repository;
using AutoMapper;

namespace APCapstoneProject.Service
{
    public class ClientUserService: IClientUserService
    {
        private readonly IClientUserRepository _clientUserRepo;
        private readonly IMapper _mapper;

        public ClientUserService(IClientUserRepository clientUserRepo, IMapper mapper)
        {
            _clientUserRepo = clientUserRepo;
            _mapper = mapper;
        }

        public async Task<ReadClientUserDto> CreateClientUserAsync(CreateClientUserDto dto, int creatorBankUserId)
        {
            var clientUser = _mapper.Map<ClientUser>(dto);
            clientUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            clientUser.UserRoleId = (int)Role.CLIENT_USER;
            clientUser.BankUserId = creatorBankUserId;
            clientUser.StatusId = 0; // pending
            clientUser.IsActive = true;

            await _clientUserRepo.AddClientUserAsync(clientUser);

            var created = await _clientUserRepo.GetClientUserByIdAsync(clientUser.UserId);
            return _mapper.Map<ReadClientUserDto>(created);
        }

        public async Task<IEnumerable<ReadClientUserDto>> GetClientsForBankUserAsync(int bankUserId)
        {
            var clients = await _clientUserRepo.GetClientsByBankUserIdAsync(bankUserId);
            return _mapper.Map<IEnumerable<ReadClientUserDto>>(clients);
        }

        public async Task<ReadClientUserDto?> GetClientForBankUserAsync(int clientId, int bankUserId)
        {
            var client = await _clientUserRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            return client == null ? null : _mapper.Map<ReadClientUserDto>(client);
        }

        public async Task<ReadClientUserDto?> UpdateClientUserAsync(int clientId, UpdateClientUserDto dto, int bankUserId)
        {
            var existing = await _clientUserRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _clientUserRepo.UpdateClientUserAsync(existing);

            var updated = await _clientUserRepo.GetClientUserByIdAsync(existing.UserId);
            return _mapper.Map<ReadClientUserDto>(updated);
        }

        public async Task<bool> DeleteClientUserAsync(int clientId, int bankUserId)
        {
            var client = await _clientUserRepo.GetClientByBankUserIdAsync(clientId, bankUserId);
            if (client == null) return false;

            return await _clientUserRepo.DeleteClientUserAsync(clientId);
        }
    }
}
