using APCapstoneProject.Data;
using APCapstoneProject.Model;

namespace APCapstoneProject.Repository
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
