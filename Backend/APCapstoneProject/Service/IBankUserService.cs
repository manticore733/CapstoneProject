using APCapstoneProject.DTO.User;
using APCapstoneProject.DTO.User.BankUser;
using APCapstoneProject.DTO.User.ClientUser;

namespace APCapstoneProject.Service
{
    public interface IBankUserService
    {
        //CRUD operations for Bank Users
        Task<ReadBankUserDto> CreateBankUserAsync(CreateBankUserDto dto);
        Task<ReadBankUserDto?> UpdateBankUserAsync(int id, UpdateBankUserDto dto);
        Task<IEnumerable<ReadBankUserDto>> GetAllBankUsersAsync();
        Task<ReadBankUserDto?> GetBankUserByIdAsync(int id);
        Task<bool> DeleteBankUserAsync(int id);

        //Business logic: Approve or reject Client Users
        Task<ReadClientUserDto?> ApproveClientUserAsync(int clientUserId, int bankUserId, ClientApprovalDto dto);

    }
}
