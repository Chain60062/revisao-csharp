using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Revisao.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Revisao.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public UserController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User userCredentials)
    {
        var existingUser = _userRepository
            .GetUserByEmailAndPwd(userCredentials.Email, userCredentials.Password);

        if (existingUser is null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        var token = GenerateToken(existingUser);
        return Ok(token);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User usuario)
    {
        _userRepository.Register(usuario);
        return Created("", usuario);
    }

    [HttpGet("listar")]
    public IActionResult GetUsers()
    {
        return Ok(_userRepository.GetUsers());
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var chave = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

        var signature = new SigningCredentials(
            new SymmetricSecurityKey(chave),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signature
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}