using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Context;

var builder = WebApplication.CreateBuilder(args); // Cria o builder da aplicação

builder.Services.AddControllers(); // Adiciona suporte a controllers

var key = "A320EFBA-E00C-4344-BCDB-A54B93D571A5"; // Chave secreta para JWT

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Ativa autenticação JWT
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Não valida o emissor
            ValidateAudience = false, // Não valida o público
            ValidateLifetime = true, // Valida tempo de expiração
            ValidateIssuerSigningKey = true, // Valida a chave
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // Define a chave
        };

        opt.Events = new JwtBearerEvents // Eventos para log
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT ERROR] Token inválido: {context.Exception.Message}"); // Log de falha
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[JWT OK] Token válido"); // Log de sucesso
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("[JWT CHALLENGE] Acesso negado. Token não enviado ou inválido."); // Log de desafio
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(); // Ativa autorização

builder.Services.AddDbContext<AppDbContext>(options => // Configura banco InMemory
    options.UseInMemoryDatabase("MyDataBase")
);

builder.Services.AddEndpointsApiExplorer(); // Suporte a endpoints no Swagger

builder.Services.AddSwaggerGen(c => // Configura Swagger
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Arq. Desenv. de APIS para BackEnd", // Título da API
        Version = "v1", // Versão da API
        Description = "API desenvolvida com .NET 9 e autenticação JWT" // Descrição
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme // Define esquema de segurança
    {
        Description = "JWT Authorization header usando o esquema Bearer. Digite 'Bearer' [espaço] e então seu token.", // Instruções
        Name = "Authorization", // Nome do header
        In = ParameterLocation.Header, // Local do header
        Type = SecuritySchemeType.ApiKey, // Tipo do esquema
        Scheme = "Bearer" // Esquema
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement() // Requer uso do token
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, // Tipo: segurança
                    Id = "Bearer" // ID do esquema
                },
            },
            new string[] {} // Sem escopos adicionais
        }
    });
});

var app = builder.Build(); // Constrói o app

using (var scope = app.Services.CreateScope()) // Cria escopo para banco
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Obtém contexto
    db.Database.EnsureCreated(); // Garante criação do banco
}

if (app.Environment.IsDevelopment()) // Verifica se é desenvolvimento
{
    app.UseSwagger(); // Ativa Swagger
    app.UseSwaggerUI(options => // Configura UI do Swagger
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Arq. Desenv. de APIS para BackEnd v1"); // URL do Swagger
        //options.RoutePrefix = string.Empty; // Swagger na raiz (opcional)
    });
}

app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.UseAuthentication(); // Ativa middleware de autenticação
app.UseAuthorization(); // Ativa middleware de autorização
app.MapControllers(); // Mapeia os controllers

app.Run(); // Executa a aplicação
