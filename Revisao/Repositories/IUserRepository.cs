using Revisao.Models;

namespace Revisao.Repositories;

public interface IUserRepository
{
    void Register(User user);
    List<User> GetUsers();
    User? GetUserByEmailAndPwd(string email, string password);
}
