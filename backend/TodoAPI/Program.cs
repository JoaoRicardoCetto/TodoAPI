using Microsoft.EntityFrameworkCore;
using TodoAPI.Application.Data;
using TodoAPI.Application.Repository;
using TodoAPI.Application.Service;
using TodoAPI.Domain;

// Ponto de entrada da aplicação ASP.NET Core.
// Este arquivo configura e inicializa a aplicação web, incluindo
// serviços, middleware e pipeline de requisições.

// Conceitos de C# e .NET aplicados aqui:
// - Top-level statements: sintaxe simplificada para programas (C# 9+)
// - Dependency Injection: configuração de serviços no container DI
// - Middleware pipeline: configuração da ordem de processamento de requisições
// - Environment configuration: configurações específicas por ambiente
// - CORS: Cross-Origin Resource Sharing para permitir requisições de outros domínios
// - Swagger/OpenAPI: documentação automática da API
// - Service lifetimes: Singleton, Scoped, Transient
// - Builder pattern: configuração fluente da aplicação

// Cria o builder da aplicação web
var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços da aplicação
builder.Services.AddControllers(); // Adiciona suporte a controllers MVC
builder.Services.AddEndpointsApiExplorer(); // Adiciona suporte a endpoints mínimos
builder.Services.AddSwaggerGen(); // Adiciona geração automática de documentação com o Swagger

// Configuração de CORS (Cross-Origin Resource Sharing).
// CORS permite que aplicações web em um domínio acessem recursos de outro domínio.
// Neste caso, está configurado para permitir requisições de uma aplicação Vue.js.
#region CORS_CONFIG
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.AllowAnyOrigin() // Permite requisições de qualquer origem
                  .AllowAnyHeader() // Permite qualquer cabeçalho HTTP
                  .AllowAnyMethod(); // Permite qualquer método HTTP (GET, POST, etc.)
        });
});
#endregion

// Configuração do banco de dados PostgreSQL
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de Dependency Injection
builder.Services.AddScoped<IBaseRepository<Todo>, TodoDbRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Constrói a aplicação
var app = builder.Build();

// Configuração do pipeline de middleware.
// O middleware é executado na ordem em que é configurado.
// Cada middleware pode processar a requisição antes de passar para o próximo.

// Configuração específica para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilita o middleware do Swagger
    app.UseSwaggerUI(); // Habilita a interface do Swagger UI
}

// Configuração do pipeline de middleware (ordem importa!)
app.UseCors("AllowVueApp"); // CORS deve vir antes de outros middlewares
app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.UseAuthorization(); // Middleware de autorização
app.MapControllers(); // Mapeia os controllers para rotas

// Inicia a aplicação e fica escutando requisições
app.Run();
