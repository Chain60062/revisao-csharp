public class User
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.USER;//padrao cria um user
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
