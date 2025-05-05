using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Revisao.Repositories;
using Revisao.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Container DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
// JWT Config
var chaveJwt = builder.Configuration["JwtSettings:SecretKey"];
//MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDataContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,                // Ignora o emissor do token
            ValidateAudience = false,              // Ignora o público do token
            ValidateLifetime = true,               // Verifica se o token está expirado
            //ClockSkew = TimeSpan.Zero,             // opcional: remove tolerância de 5 min
            ValidateIssuerSigningKey = true,       // Garante que a assinatura do token é válida
            IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(chaveJwt!)) // Chave secreta usada para validar o token
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
