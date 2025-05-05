# Início do Projeto

Para criar uma solução:
```bash
dotnet new sln --name NomeDaSolucao
```

Para criar uma web api nova em ASP.NET Core com o padrão MVC:
```bash
dotnet new webapi -n NomeDoProjeto --no-https --use-controllers
```

Para adicionar um projeto a solução:
```bash
dotnet sln [<SOLUTION_FILE>] add [--in-root] [-s|--solution-folder <PATH>]
```

Adaptado para os nomes acima: 
```bash
dotnet sln NomeDaSolucao.sln add NomeDoProjeto
```
Caso isso dê erro, tente especificar o caminho completo do `.csproj` do seu projeto, algo como `NomeDoProjeto/NomeDoProjeto.csproj`.

Crie um `.gitignore`
``bash
dotnet new gitignore
``

Depois, adicione os pacotes do mysql, ef core e jwt respectivamente ao projeto:
```bash
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.*
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.*
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version: 8.*
```

---
# EF
Para baixar o cli do ef use:
```bash
dotnet tool install --global dotnet-ef --version "8.0.*" 
```
Se por algum motivo o comando acima não funcionar, saiba que no dia 28/04 a versão mais recente do `dotnet-ef` é a 8.0.15.

E se por algum motivo o comando `dotnet ef` não estiver funcionando mesmo depois de fechar e abrir o terminal, tente usar as ferramntas localmente, da documentação:

>Local tools are stored in the NuGet global directory, whatever you've set that to be. There are shim files in `$HOME/.dotnet/toolResolverCache` for each local tool that point to where the tools are within that location.

>References to local tools are added to a _dotnet-tools.json_ file in a _.config_ directory under the current directory. If a manifest file doesn't exist yet, create it by using the `--create-manifest-if-needed` option or by running the following command:
```bash
dotnet new tool-manifest
```

Use --tool-path para alterar o caminho se precisar:
> `--tool-path` tools
>
>Tools with explicit tool paths are stored wherever you specified the `--tool-path` parameter to point to. They're stored in the same way as global tools: an executable binary with the actual binaries in a sibling `.store` directory. 

---
## Migrations

```bash
dotnet ef migrations add NomeDaMigration
```

---
## Database
```bash
dotnet ef database update
```

```bash
dotnet ef database drop
```

---
# JWT
Primeiro, gere uma chave para a assinatura:
```powershell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

Ou, se estiver em um ambiente unix(bash, zsh, git bash...):
```bash
openssl rand -base64 32
```

Agora, vá para o `appsettings.json` e acrescente a seguinte secção:
```json
"JwtSettings": { 
	"SecretKey": "o-valor-da-chave-aqui" 
}
```

Em `Program.cs` configure o JWT:
```c#
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var chaveJwt = builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,                // Ignora o emissor do token
            ValidateAudience = false,              // Ignora o público do token
            ValidateLifetime = true,               // Verifica se o token está expirado
            ClockSkew = TimeSpan.Zero,             // opcional: remove tolerância de 5 min
            ValidateIssuerSigningKey = true,       // Garante que a assinatura do token é válida
            IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(chaveJwt!)) // Chave secreta usada para validar o token
        };
    });

builder.Services.AddAuthorization();
```
E adicione:
```c#
app.UseAuthentication(); 
app.UseAuthorization();
```

Um controller de usuários com login simples:
```c#
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
```

---
# Banco de Dados(MySQL e PostgreSQL)
Primeiro abra o XAMPP e inicie o MySQL e Apache.

Crie um DbContext:
```c#
public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions options) :
        base(options)
    { }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Usuarios { get; set; }
}
```

Em `appsettings.json`, adicione:
```json
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;database=revisao;user=root;password=mysqlpassword123"
  },
```

Em `Program.cs` adicione:
```c#
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```
Se o MySQL não funcionar, ou tiver algum problema, use o PostgreSQL:
```c#
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

---
# Tipos
## Decimal

Literais do tipo `decimal` usam o sufixo `m` ou `M`:
```c#
decimal myDecimal = 14m
```

---
# Git
## Remover entradas utilizando `git config`
You can use the `--unset` flag of `git config` to do this like so:

```bash
git config --global --unset user.name
git config --global --unset user.email
```

If you have more variables for one config you can use:

```bash
git config --global --unset-all user.name
```

---
# Erros Comuns 
## Cors
```c#
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## Referências Cíclicas

Existem algumas opções, a mais fácil para prova é ignorar ciclos globalmente:
```c#
using System.Text.Json.Serialization;

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

//se o código acima não funcionar, tente
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});
```
## Usando DTOS
Instead of exposing your entity models directly, use **DTOs or ViewModels** that flatten or reshape the data:
```c#
public class ParentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ChildDto> Children { get; set; }
}

public class ChildDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```
## JsonIgnore
If one side of the navigation is not necessary during serialization (e.g., you don’t need to return the parent in the child object), you can ignore it:

```c#
public class Parent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Child> Children { get; set; }
}

public class Child
{
    public int Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore] // Prevents infinite loop
    public Parent Parent { get; set; }
}
```
Use `System.Text.Json.Serialization.JsonIgnore` if you're using the default .NET serializer.

---
## Métodos HTTP(Ok, Unauthorized, Created, etc...)
As vezes, pode ocorrer dos métodos do título não serem identificados corretamente, saiba que todos vêm da class ControllerBase, então ControllerBase.Ok, ControllerBase.Unauthorized...

---

## Porta do MySQL
Verifique a porta do mysql, geralmente nos pcs da faculdade ele roda na porta 3307. 
