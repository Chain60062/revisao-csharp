using Revisao.Data;
using Revisao.Models;
namespace Revisao.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDataContext _context;
    public UserRepository(AppDataContext context)
    {
        _context = context;
    }

    public User? GetUserByEmailAndPwd(string email, string senha)
    {
        User? existingUser =
            _context.Usuarios.FirstOrDefault
            (x => x.Email == email && x.Password == senha);
        return existingUser;
    }

    public void Register(User user)
    {
        _context.Usuarios.Add(user);
        _context.SaveChanges();
    }

    public List<User> GetUsers()
    {
        return _context.Usuarios.ToList();
    }
}
