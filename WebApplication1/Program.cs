using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApplication1.Context;

var builder = WebApplication.CreateBuilder(args);

// Adicionando os serviços de conteiner.
builder.Services.AddControllers();

// Configuração JWT
var key = "A320EFBA-E00C-4344-BCDB-A54B93D571A5";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };


        opt.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT ERROR] Token inválido: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[JWT OK] Token válido");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("[JWT CHALLENGE] Acesso negado. Token não enviado ou inválido.");
                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddAuthorization();

// Adicionando o suporte ao Entity Framework Core com InMemory Database.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("MyDataBase")
);

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Arq. Desenv. de APIS para BackEnd",
        Version = "v1",
        Description = "API desenvolvida com .NET 9 e autenticação JWT"
    });

    // Configuração do JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Digite 'Bearer' [espaço] e então seu token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Chamada para aplicar o seeding para o banco de dados.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configurando o PIPELINE http.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Arq. Desenv. de APIS para BackEnd v1");
        //options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();