# 🚀 TodoAPI - Tutorial para Iniciantes

Este tutorial ensina como criar uma API REST simples para gerenciar tarefas (Todo) usando **.NET 8**.

> **📚 Não conhece os conceitos básicos?** Leia primeiro o arquivo [conceitosBasicos.md](conceitosBasicos.md) para entender termos como API, Backend, Banco de Dados, REST e outros conceitos fundamentais. 

## 📋 Índice

- [O que vamos construir?](#-o-que-vamos-construir)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Como funciona a arquitetura?](#-como-funciona-a-arquitetura)
- [Passo a Passo](#-passo-a-passo)
- [Configuração do PostgreSQL](#-configuração-do-postgresql)
- [Como executar?](#-como-executar)
- [Testando a API](#-testando-a-api)
- [Conceitos Básicos](#-conceitos-básicos)

## 🎯 O que vamos construir?

Uma API REST que permite:
- ✅ Criar tarefas
- ✅ Listar todas as tarefas
- ✅ Buscar uma tarefa específica
- ✅ Atualizar uma tarefa
- ✅ Marcar tarefa como completa/incompleta
- ✅ Remover uma tarefa

## 🏗️ Como funciona a arquitetura?

Nossa aplicação tem 4 partes principais:

```
📁 TodoAPI/
├── 🎯 Domain/          # O que é uma tarefa
├── 🔧 Application/     # Como gerenciar tarefas
├── 🏗️ Infrastructure/  # Como acessar dados
└── 🌐 Presentation/    # Como acessar via internet
```

### O que cada parte faz:
- **Domain**: Define o que é uma tarefa (id, descrição, se está completa)
- **Application**: Contém as regras de negócio (criar, buscar, atualizar, remover)
- **Infrastructure**: Implementa acesso a dados (repositories, banco de dados, migrations)
- **Presentation**: Cria os endpoints da API (GET, POST, PUT, DELETE)

## 📂 Estrutura do Projeto

```
TodoAPI/
├── Domain/
│   └── Todo.cs                    # O que é uma tarefa
├── Application/
│   └── Service/
│       ├── ITodoService.cs        # O que podemos fazer com tarefas
│       └── TodoService.cs         # Como fazer essas operações
├── Infrastructure/
│   ├── Data/
│   │   └── TodoDbContext.cs       # Configuração do banco de dados
│   ├── Repository/
│   │   ├── IBaseRepository.cs     # Como salvar/buscar dados
│   │   └── TodoDbRepository.cs    # Repositório para PostgreSQL
│   └── Migrations/                # Migrações do banco de dados
├── Presentation/
│   ├── Controllers/
│   │   └── TodoController.cs      # Endpoints da API
│   └── Dtos/
│       └── TodoDto.cs             # Dados que vêm da internet
├── Program.cs                     # Configuração da aplicação
└── TodoAPI.csproj                # Arquivo do projeto
```

## 🛠️ Passo a Passo

### Passo 1: Criar o Projeto

```bash
dotnet new webapi -n TodoAPI
cd TodoAPI
```

### Passo 2: Definir o que é uma Tarefa

Crie a classe `Todo` na pasta `Domain/`:

```csharp
namespace TodoAPI.Domain
{
    // Representa uma tarefa na aplicação
    public class Todo
    {
        // ID único da tarefa
        public Guid Id { get; set; } = Guid.NewGuid();
        
        // Descrição da tarefa
        public string Descricao { get; set; } = string.Empty;
        
        // Se a tarefa está completa ou não
        public bool Completo { get; set; } = false;
    }
}
```

**O que faz:** Define o que é uma tarefa (id, descrição, se está completa)

### Passo 3: Como Salvar e Buscar Dados

Crie a interface `IBaseRepository` na pasta `Infrastructure/Repository/`:

```csharp
namespace TodoAPI.Infrastructure.Repository
{   
    // Define as operações básicas para trabalhar com dados
    public interface IBaseRepository<T>
    {
        // Buscar todas as entidades
        Task<IEnumerable<T>> GetAllAsync();
        
        // Buscar uma entidade pelo ID
        Task<T?> GetByIdAsync(Guid id);
        
        // Criar uma nova entidade
        Task CreateAsync(T entity);
        
        // Atualizar uma entidade existente
        Task UpdateAsync(T entity);
        
        // Remover uma entidade pelo ID
        Task DeleteAsync(Guid id);
    }
}
```

**O que faz:** Define como podemos salvar, buscar, atualizar e remover dados

### Passo 4: Configurar o Banco de Dados

Crie o `TodoDbContext` na pasta `Infrastructure/Data/`:

```csharp
using Microsoft.EntityFrameworkCore;
using TodoAPI.Domain;

namespace TodoAPI.Infrastructure.Data
{
    // Contexto do banco de dados para gerenciar as tarefas
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        // Define a tabela de tarefas no banco
        public DbSet<Todo> Todos { get; set; }

        // Configuração do modelo de dados
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar a entidade Todo
            modelBuilder.Entity<Todo>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Descricao)
                      .IsRequired()
                      .HasMaxLength(500);
                entity.Property(t => t.Completo)
                      .IsRequired()
                      .HasDefaultValue(false);
                entity.ToTable("Todos");
            });
        }
    }
}
```

**O que faz:** Configura o Entity Framework para trabalhar com PostgreSQL

### Passo 5: Implementar o Repositório

Implemente o `TodoDbRepository` na pasta `Infrastructure/Repository/`:

```csharp
using Microsoft.EntityFrameworkCore;
using TodoAPI.Infrastructure.Data;
using TodoAPI.Domain;

namespace TodoAPI.Infrastructure.Repository
{
    // Implementa o repositório para salvar tarefas no banco de dados
    public class TodoDbRepository : IBaseRepository<Todo>
    {
        private readonly TodoDbContext _context;

        public TodoDbRepository(TodoDbContext context)
        {
            _context = context;
        }

        // Buscar todas as tarefas
        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        // Buscar uma tarefa pelo ID
        public async Task<Todo?> GetByIdAsync(Guid id)
        {
            return await _context.Todos.FindAsync(id);
        }

        // Adicionar uma nova tarefa
        public async Task CreateAsync(Todo entity)
        {
            _context.Todos.Add(entity);
            await _context.SaveChangesAsync();
        }

        // Atualizar uma tarefa existente
        public async Task UpdateAsync(Todo entity)
        {
            _context.Todos.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Remover uma tarefa
        public async Task DeleteAsync(Guid id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

**O que faz:** Implementa as operações de banco de dados usando Entity Framework

### Passo 6: O que Podemos Fazer com Tarefas

Crie a interface `ITodoService` na pasta `Application/Service/`:

```csharp
using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    // Define as operações de negócio para gerenciar tarefas
    public interface ITodoService
    {
        // Buscar todas as tarefas
        Task<IEnumerable<Todo>> GetAllAsync();
        
        // Buscar uma tarefa pelo ID
        Task<Todo?> GetByIdAsync(Guid id);
        
        // Criar uma nova tarefa
        Task<Todo> CreateAsync(string descricao, bool completo = false);
        
        // Atualizar uma tarefa existente
        Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo);
        
        // Alternar o status de uma tarefa (completa/incompleta)
        Task<bool> ToggleCompleteAsync(Guid id);
        
        // Remover uma tarefa
        Task<bool> DeleteAsync(Guid id);
    }
}
```

**O que faz:** Define as operações que podemos fazer com tarefas

### Passo 7: Como Fazer Essas Operações

Implemente o `TodoService` na pasta `Application/Service/`:

```csharp
using TodoAPI.Infrastructure.Repository;
using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    // Implementa a lógica de negócio para gerenciar tarefas
    public class TodoService : ITodoService
    {
        // Repositório para salvar e buscar tarefas
        private readonly IBaseRepository<Todo> _repo;

        // Construtor que recebe o repositório
        public TodoService(IBaseRepository<Todo> repo)
        {
            _repo = repo;
        }

        // Buscar todas as tarefas
        public Task<IEnumerable<Todo>> GetAllAsync() => _repo.GetAllAsync();

        // Buscar uma tarefa pelo ID
        public Task<Todo?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        // Criar uma nova tarefa
        public async Task<Todo> CreateAsync(string descricao, bool completo = false)
        {
            // Validar se a descrição não está vazia
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição obrigatória", nameof(descricao));

            // Criar a tarefa
            var todo = new Todo
            {
                Descricao = descricao.Trim(),
                Completo = completo
            };

            // Salvar no repositório
            await _repo.CreateAsync(todo);
            return todo;
        }

        // Atualizar uma tarefa existente
        public async Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo)
        {
            // Verificar se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            // Atualizar descrição se fornecida
            if (descricao != null)
            {
                if (string.IsNullOrWhiteSpace(descricao))
                    throw new ArgumentException("Descrição não pode ficar vazia", nameof(descricao));
                existing.Descricao = descricao.Trim();
            }

            // Atualizar status se fornecido
            if (completo.HasValue)
                existing.Completo = completo.Value;

            // Salvar as alterações
            await _repo.UpdateAsync(existing);
            return true;
        }

        // Alternar o status de uma tarefa
        public async Task<bool> ToggleCompleteAsync(Guid id)
        {
            // Verificar se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            // Alternar o status
            existing.Completo = !existing.Completo;
            
            // Salvar a alteração
            await _repo.UpdateAsync(existing);
            return true;
        }

        // Remover uma tarefa
        public async Task<bool> DeleteAsync(Guid id)
        {
            // Verificar se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            
            // Remover do repositório
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
```

**O que faz:** Implementa as regras de negócio (validações, lógica)

### Passo 8: Dados que Vêm da Internet

Crie o `TodoDto` na pasta `Presentation/Dtos/`:

```csharp
namespace TodoAPI.Controller.Dtos
{
    // DTO para enviar dados de tarefas pela API
    // Funciona tanto para criar quanto para atualizar tarefas
    public class TodoDto
    {
        // Descrição da tarefa
        public string? Descricao { get; set; }
        
        // Se a tarefa está completa ou não
        public bool? Completo { get; set; }
    }
}
```

**O que faz:** Define os dados que vêm da internet (JSON)

### Passo 9: Endpoints da API

Implemente o `TodoController` na pasta `Presentation/Controllers/`:

```csharp
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.Service;
using TodoAPI.Controller.Dtos;

namespace TodoAPI.Controller.Controllers
{
    // Gerencia as operações da API REST para tarefas
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        // Serviço para gerenciar tarefas
        private readonly ITodoService _service;

        public TodoController(ITodoService service)
        {
            _service = service;
        }

        // Buscar todas as tarefas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // Buscar uma tarefa pelo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var todo = await _service.GetByIdAsync(id);
            return todo is null ? NotFound() : Ok(todo);
        }

        // Criar uma nova tarefa
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoDto dto)
        {
            try
            {
                // Verificar se a descrição foi fornecida
                if (string.IsNullOrWhiteSpace(dto.Descricao))
                    return BadRequest(new { message = "Descrição é obrigatória" });
                
                // Criar a tarefa
                var todo = await _service.CreateAsync(dto.Descricao, dto.Completo ?? false);
                
                // Retornar a tarefa criada
                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Atualizar uma tarefa existente
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto.Descricao, dto.Completo);
                return ok ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Alternar o status de uma tarefa
        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var ok = await _service.ToggleCompleteAsync(id);
            return ok ? Ok() : NotFound();
        }

        // Remover uma tarefa
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
```

**O que faz:** Cria os endpoints da API (GET, POST, PUT, PATCH, DELETE)

### Passo 10: Configurar a Aplicação

Configure o `Program.cs`:

```csharp
using TodoAPI.Infrastructure.Data;
using TodoAPI.Infrastructure.Repository;
using TodoAPI.Application.Service;
using TodoAPI.Domain;

// Cria o builder da aplicação web
var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços da aplicação
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração de CORS para permitir requisições de outros domínios
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configuração de Dependency Injection
builder.Services.AddSingleton<IBaseRepository<Todo>, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

// Constrói a aplicação
var app = builder.Build();

// Configuração específica para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configuração do pipeline de middleware
app.UseCors("AllowVueApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Inicia a aplicação
app.Run();
```

**O que faz:** Configura a aplicação, registra serviços e inicializa o servidor

## 🗄️ Configuração do PostgreSQL

### 📦 Instalação do PostgreSQL

#### **1. Download e Instalação**

1. **Acesse:** https://www.postgresql.org/download/windows/
2. **Clique em:** "Download the installer"
3. **Execute o instalador** como administrador
4. **Selecione componentes:**
   - ✅ PostgreSQL Server
   - ✅ pgAdmin 4 (interface gráfica)
   - ✅ Command Line Tools

#### **2. Configuração Inicial**

1. **Senha do usuário postgres:**
   ```
   Digite: postgres
   ```

2. **Porta:**
   ```
   Mantenha: 5432
   ```

3. **Localização:**
   ```
   Selecione: Portuguese, Brazil
   ```

### 🔧 Configuração via pgAdmin

#### **1. Abrir pgAdmin**

- **Windows:** Procure por "pgAdmin 4" no menu iniciar
- **Primeira execução:** Digite a senha: postgres

#### **2. Conectar ao Servidor**

1. **Clique com botão direito** em "Servers" → "Create" → "Server"
2. **General Tab:**
   ```
   Name: TodoAPILocal
   ```
3. **Connection Tab:**
   ```
   Host name/address: localhost
   Port: 5432
   Maintenance database: postgres
   Username: postgres
   Password: postgres
   ```
4. **Clique em "Save"**

#### **3. Criar o Banco de Dados**

1. **Clique com botão direito** em "TodoAPILocal" → "Create" → "Database"
2. **Database:**
   ```
   Database: TodoDB
   Owner: postgres
   ```
3. **Clique em "Save"**

### 🚀 Criação de Migrações

#### **1. Instalar Entity Framework Tools**
Abra o terminal do Visual Studio (Ctrl + ") ou use o PowerShell e digite os comandos:
```bash
# Instalar ferramentas do EF Core globalmente
dotnet tool install --global dotnet-ef
```

#### **2. Adicionar Pacotes NuGet**

```bash
# Adicionar pacotes necessários
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

#### **3. Criar Migração Inicial**

```bash
# Criar a primeira migração
dotnet ef migrations add InitialCreate
```

**O que acontece:**
- ✅ Cria pasta `Migrations/` no projeto
- ✅ Gera arquivos de migração com a estrutura da tabela `Todos`
- ✅ Não aplica ainda no banco (apenas cria os arquivos)

#### **4. Aplicar Migração no Banco**

```bash
# Aplicar migrações no banco de dados
dotnet ef database update
```

**O que acontece:**
- ✅ Cria a tabela `Todos` no banco `TodoDB`
- ✅ Aplica a estrutura definida no `TodoDbContext`
- ✅ Banco fica pronto para uso

#### **5. Verificar no pgAdmin**

1. **Expanda:** TodoAPI Local → Databases → TodoDB → Schemas → public → Tables
2. **Você deve ver:** A tabela `todos` criada
3. **Clique com botão direito** em `todos` → "View/Edit Data" → "All Rows"

### 🔄 CURIOSIDADE - Comandos de Migração Úteis

#### **Criar Nova Migração**
```bash
# Após fazer alterações no TodoDbContext ou entidade Todo
dotnet ef migrations add NomeDaMigracao
```

#### **Aplicar Migrações Pendentes**
```bash
# Aplicar todas as migrações não aplicadas
dotnet ef database update
```

#### **Ver Status das Migrações**
```bash
# Listar todas as migrações
dotnet ef migrations list
```

#### **Remover Última Migração**
```bash
# Remove a última migração criada (se não aplicada)
dotnet ef migrations remove
```

#### **Resetar Banco (CUIDADO!)**
```bash
# Remove todas as migrações e recria o banco
dotnet ef database drop
dotnet ef database update
```


## ⚙️ Como executar?

### Pré-requisitos
- .NET 8 SDK
- PostgreSQL instalado localmente
- pgAdmin 4 configurado


### 2. Configurar o Projeto

```bash
# Instalar ferramentas EF Core
dotnet tool install --global dotnet-ef

# Adicionar pacotes NuGet
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

### 3. Configurar String de Conexão

Edite o arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TodoDB;Username=postgres;Password=postgres"
  }
}
```


### 6. Executar a Aplicação

```bash
# Restaurar dependências
dotnet restore

# Compilar o projeto
dotnet build

# Executar a aplicação
dotnet run

# Executar com hot reload (desenvolvimento)
dotnet watch run
```

### 7. Acessar a API

A aplicação estará disponível em:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`


## 🔗 Testando a API

| Método | Endpoint | Descrição | Parâmetros |
|--------|----------|-----------|------------|
| `GET` | `/api/Todo` | Listar todas as tarefas | - |
| `GET` | `/api/Todo/{id}` | Buscar tarefa por ID | `id` (Guid) |
| `POST` | `/api/Todo` | Criar nova tarefa | Body: `TodoDto` |
| `PUT` | `/api/Todo/{id}` | Atualizar tarefa | `id` (Guid) + Body: `TodoDto` |
| `PATCH` | `/api/Todo/{id}/toggle` | Alternar status da tarefa | `id` (Guid) |
| `DELETE` | `/api/Todo/{id}` | Excluir tarefa | `id` (Guid) |


## 🎓 O que aprendemos?

### **Conceitos Básicos**
- ✅ **API REST**: Endpoints para criar, ler, atualizar e deletar
- ✅ **Arquitetura em Camadas**: Separar responsabilidades
- ✅ **Dependency Injection**: Injeção de dependências
- ✅ **Async/Await**: Operações assíncronas

### **Padrões Utilizados**
- ✅ **Repository Pattern**: Abstração do acesso a dados
- ✅ **Service Layer**: Lógica de negócio
- ✅ **DTO Pattern**: Transferência de dados
- ✅ **Entity Framework Core**: ORM para PostgreSQL

### **Conceitos de C#**
- ✅ **Classes e Interfaces**: Organização do código
- ✅ **Nullable Types**: Valores opcionais
- ✅ **Async/Await**: Operações não-bloqueantes
- ✅ **Exception Handling**: Tratamento de erros


**🎉 Parabéns!** Você criou sua primeira API REST com .NET 8!