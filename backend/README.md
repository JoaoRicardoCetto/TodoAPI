# 🚀 TodoAPI - Tutorial para Iniciantes

Este tutorial ensina como criar uma API REST simples para gerenciar tarefas (Todo) usando **.NET 8**.

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
├── 🏗️ Infrastructure/  # Como acessar dados
├── 🔧 Application/     # Como gerenciar tarefas
└── 🌐 Presentation/    # Como acessar via internet
```

### O que cada parte faz:
- **Domain**: Define o que é uma tarefa (id, descrição, se está completa)
- **Infrastructure**: Implementa acesso a dados (repositories, banco de dados, migrations)
- **Application**: Contém as regras de negócio (criar, buscar, atualizar, remover)
- **Presentation**: Cria os endpoints da API (GET, POST, PUT, DELETE)

## 📂 Estrutura do Projeto

```
TodoAPI/
├── Domain/
│   └── Todo.cs                    # O que é uma tarefa
├── Infrastructure/
│   ├── Data/
│   │   └── TodoDbContext.cs       # Configuração do banco de dados
│   ├── Repository/
│   │   ├── IBaseRepository.cs     # Como salvar/buscar dados
│   │   └── TodoDbRepository.cs    # Repositório para PostgreSQL
│   └── Migrations/                # Migrações do banco de dados
├── Application/
│   └── Service/
│       ├── ITodoService.cs        # O que podemos fazer com tarefas
│       └── TodoService.cs         # Como fazer essas operações
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

1. Abra o **Visual Studio**.  
2. Clique em **Criar um novo projeto**.  
3. Na barra de pesquisa, digite **ASP.NET Core Vazio**, selecione essa opção e avance.  
4. No campo **Nome do projeto**, digite `TodoAPI` e clique em **Próximo**.  
5. Em **Estrutura**, escolha **.NET 8.0 (Suporte a Longo Prazo)**.  
6. Clique em **Criar** para finalizar a configuração inicial do projeto.  

#### Instalar pacotes necessários
Após criar o projeto, é necessário adicionar alguns pacotes para o funcionamento da aplicação.  

1. Na barra superior, clique em **Projeto > Gerenciar Pacotes NuGet**.  
2. Vá até a guia **Procurar** e instale os seguintes pacotes:
   - `Microsoft.EntityFrameworkCore.Design`  
   - `Npgsql.EntityFrameworkCore.PostgreSQL`  
   - `Microsoft.EntityFrameworkCore`  
   - `Swashbuckle.AspNetCore`

---

### Passo 2: Definir o que é um To Do

1. No projeto **TodoAPI**, clique com o botão direito e selecione **Adicionar > Nova Pasta**.  
2. Nomeie a pasta como **Domain**.  
3. Dentro da pasta **Domain**, clique com o botão direito e selecione **Adicionar > Novo Item**.  
4. Crie uma nova classe chamada `Todo.cs`:  

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

1. No projeto **TodoAPI**, clique com o botão direito e selecione **Adicionar > Nova Pasta**.  
2. Nomeie a pasta como **Infrastructure**.  
3. Dentro de **Infrastructure**, crie uma nova pasta chamada **Repositories**.  
4. Na pasta **Repositories**, adicione uma nova *interface* chamada `IBaseRepository.cs`. 

```csharp
namespace TodoAPI.Infrastructure.Repositories
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
**O que faz:** Essa interface irá definir as operações que podemos fazer com nosso banco de dados (salvar, buscar, atualizar e remover dados)


### Passo 4: Configurar o Banco de Dados

1. Dentro de **Infrastructure**, crie uma nova pasta chamada **Data**.  
2. Na pasta **Data**, adicione uma nova *classe* chamada `TodoDbContext.cs`.

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
**O que faz:** Essa classe será responsável por configurar o Entity Framework para trabalhar com nosso banco de dados PostgreSQL, além de fazer o mapeamento da Entidade ToDo para um modelo de banco de dados.


### Passo 5: Implementar o Repositório
1. Dentro de **Infrastructure/Repositories**, adicione uma nova *classe* chamada `TodoDbRepository.cs`.

```csharp
using Microsoft.EntityFrameworkCore;
using TodoAPI.Infrastructure.Data;
using TodoAPI.Domain;

namespace TodoAPI.Infrastructure.Repositories
{
    // Implementa o repositório para salvar tarefas no banco de dados
    public class TodoDbRepository : IBaseRepository<Todo>
    {
        // Contexto do banco de dados para acessar as tabelas
        private readonly TodoDbContext _context;

        // Construtor que recebe o contexto do banco
        public TodoDbRepository(TodoDbContext context)
        {
            _context = context;
        }

        // Buscar todas as tarefas do banco
        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            return await _context.Todos.ToListAsync();
        }

        // Buscar uma tarefa específica pelo ID
        public async Task<Todo?> GetByIdAsync(Guid id)
        {
            return await _context.Todos.FindAsync(id);
        }

        // Adicionar uma nova tarefa no banco
        public async Task CreateAsync(Todo entity)
        {
            _context.Todos.Add(entity);
            await _context.SaveChangesAsync();
        }

        // Atualizar uma tarefa existente no banco
        public async Task UpdateAsync(Todo entity)
        {
            _context.Todos.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Remover uma tarefa do banco
        public async Task DeleteAsync(Guid id)
        {
            // Buscar a tarefa pelo ID
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                // Remover e salvar as alterações
                _context.Todos.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
```

**O que faz:** Implementa as operações de banco de dados usando métodos do Entity Framework

### Passo 6: O que Podemos Fazer com Tarefas
1. No projeto **TodoAPI**, clique com o botão direito e selecione **Adicionar > Nova Pasta**.  
2. Nomeie a pasta como **Application**.  
3. Dentro de **Application**, crie uma nova pasta chamada **Services**.  
4. Na pasta **Services**, adicione uma nova *interface* chamada `ITodoService.cs`. 

```csharp
using TodoAPI.Domain;

namespace TodoAPI.Application.Services
{
    // Define as operações de negócio para gerenciar tarefas
    public interface ITodoService
    {
        // Buscar todas as tarefas cadastradas
        Task<IEnumerable<Todo>> GetAllAsync();
        
        // Buscar uma tarefa específica pelo ID
        Task<Todo?> GetByIdAsync(Guid id);
        
        // Criar uma nova tarefa com descrição e status
        Task<Todo> CreateAsync(string descricao, bool completo = false);
        
        // Atualizar uma tarefa existente (descrição e/ou status)
        Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo);
        
        // Alternar o status de uma tarefa (completa/incompleta)
        Task<bool> ToggleCompleteAsync(Guid id);
        
        // Remover uma tarefa do sistema
        Task<bool> DeleteAsync(Guid id);
    }
}
```

**O que faz:** Define as operações que podemos fazer com tarefas

### Passo 7: Como Fazer Essas Operações
1. Dentro de **Application/Services**, adicione uma nova *classe* chamada `TodoService.cs`. 

```csharp
using TodoAPI.Infrastructure.Repositories;
using TodoAPI.Domain;

namespace TodoAPI.Application.Services
{
    // Implementa a lógica de negócio para gerenciar tarefas
    public class TodoService : ITodoService
    {
        // Repositório para salvar e buscar tarefas no banco
        private readonly IBaseRepository<Todo> _repo;

        // Construtor que recebe o repositório via injeção de dependência
        public TodoService(IBaseRepository<Todo> repo)
        {
            _repo = repo;
        }

        // Buscar todas as tarefas do repositório
        public Task<IEnumerable<Todo>> GetAllAsync() => _repo.GetAllAsync();

        // Buscar uma tarefa específica pelo ID
        public Task<Todo?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        // Criar uma nova tarefa com validações
        public async Task<Todo> CreateAsync(string descricao, bool completo = false)
        {
            // Validar se a descrição não está vazia
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição obrigatória", nameof(descricao));

            // Criar a nova tarefa
            var todo = new Todo
            {
                Descricao = descricao.Trim(),
                Completo = completo
            };

            // Salvar no repositório
            await _repo.CreateAsync(todo);
            return todo;
        }

        // Atualizar uma tarefa existente com validações
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

            // Salvar as alterações no repositório
            await _repo.UpdateAsync(existing);
            return true;
        }

        // Alternar o status de uma tarefa (completa/incompleta)
        public async Task<bool> ToggleCompleteAsync(Guid id)
        {
            // Verificar se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            // Alternar o status (true vira false, false vira true)
            existing.Completo = !existing.Completo;
            
            // Salvar a alteração no repositório
            await _repo.UpdateAsync(existing);
            return true;
        }

        // Remover uma tarefa do sistema
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

**O que faz:** Implementa as regras de negócio (validações, lógica).

### Passo 8: Dados que transitam entre front e back-end
1. No projeto **TodoAPI**, clique com o botão direito e selecione **Adicionar > Nova Pasta**.  
2. Nomeie a pasta como **Presentation**.  
3. Dentro de **Presentation**, crie uma nova pasta chamada **Dtos**.  
4. Na pasta **Dtos**, adicione uma nova *classe* chamada `TodoDto.cs`. 

```csharp
namespace TodoAPI.Presentation.Dtos
{
    // DTO para enviar dados de tarefas pela API
    // Funciona tanto para criar quanto para atualizar tarefas
    public class TodoDto
    {
        // Descrição da tarefa (opcional para atualizações)
        public string? Descricao { get; set; }
        
        // Se a tarefa está completa ou não (opcional para atualizações)
        public bool? Completo { get; set; }
    }
}
```

**O que faz:** Define os dados que vêm da internet. Essa será a estrutura dos JSONs que transitarão por de baixo dos panos

### Passo 9: Endpoints da API
1. Dentro de **Presentation**, crie uma nova pasta chamada **Controllers**.  
2. Na pasta **Controllers**, adicione uma nova *classe* chamada `TodoController.cs`. 

```csharp
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.Services;
using TodoAPI.Presentation.Dtos;

namespace TodoAPI.Presentation.Controllers
{
    // Gerencia as operações da API REST para tarefas
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        // Serviço para gerenciar tarefas via injeção de dependência
        private readonly ITodoService _service;

        // Construtor que recebe o serviço
        public TodoController(ITodoService service)
        {
            _service = service;
        }

        // Buscar todas as tarefas cadastradas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // Buscar uma tarefa específica pelo ID
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
                
                // Criar a tarefa usando o serviço
                var todo = await _service.CreateAsync(dto.Descricao, dto.Completo ?? false);
                
                // Retornar a tarefa criada com status 201
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
                // Tentar atualizar a tarefa
                var ok = await _service.UpdateAsync(id, dto.Descricao, dto.Completo);
                return ok ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Alternar o status de uma tarefa (completa/incompleta)
        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            // Alternar o status da tarefa
            var ok = await _service.ToggleCompleteAsync(id);
            return ok ? Ok() : NotFound();
        }

        // Remover uma tarefa do sistema
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Remover a tarefa
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
```

**O que faz:** Cria os endpoints da API (GET, POST, PUT, PATCH, DELETE) que serão responsáveis pela comunicação do back com o frontend

### Passo 10: Configurar a Aplicação

Configure o `Program.cs`:

```csharp
using TodoAPI.Infrastructure.Data;
using TodoAPI.Infrastructure.Repositories;
using TodoAPI.Application.Services;
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

// Configuração do Entity Framework com PostgreSQL
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de Dependency Injection
builder.Services.AddScoped<IBaseRepository<Todo>, TodoDbRepository>();
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

Caso já tenha o postgres instalado, siga para **o passo Configuração via pgAdmin**


## ⚙️ Pré configuração do projeto para conexão com o banco de dados

### 1. Configurar String de Conexão

Edite o arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=TodoDB;Username=postgres;Password=postgres"
  }
}
```

**O que faz:** Define como a aplicação se conecta ao banco PostgreSQL


### 📦 Instalação do PostgreSQL

Se o PostgreSQL não estiver instalado em sua máquina, siga o passo a passo abaixo para realizar a instalação.

#### **1. Download e Instalação**
1. **Acesse:** https://www.postgresql.org/download
2. Selecione seu sistema operacional
3. **Clique em:** "Download the installer"
4. **Execute o instalador** como administrador
5. **Selecione componentes:**
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
- **Primeira execução:** Digite a senha: postgres (senha que você criou na instalação)

#### **2. Conectar ao Servidor**

1. **Clique com botão direito** em "Servers" → "Register" → "Server"
2. **Aba "General":**
   ```
   Name: TodoServer
   (Nome para identificar sua conexão local)
   ```
3. **Aba "Connection":**
   ```
   Host name/address: localhost
   Port: 5432
   Maintenance database: postgres
   Username: postgres
   Password: postgres
   ```
4. **Clique em "Save"**

#### **3. Criar o Banco de Dados**

1. Ainda no PgAdmin4, **Clique com botão direito** em "TodoServer" → "Create" → "Database"
2. **Database:**
   ```
   Database: TodoDB
   Owner: postgres
   ```
3. **Clique em "Save"**

### 🚀 Criação de Migrations

#### **3. Criar Migração Inicial**

No Visual Studio, acesse o "PowerShell do Desenvolvedor" pressionando os botões **Ctrl + "**
Com o terminal aberto, digite os comandos
```bash
#Acessa a pasta do projeto
cd TodoAPI
#Compila e constrói a aplicação
dotnet build
# Criar a primeira migração
dotnet ef migrations add todoDbMigrations
```

**O que acontece:**
- ✅ Criação da pasta `Migrations/` no projeto
- ✅ Gera arquivos de migração com a estrutura da tabela `Todos`
- ✅ Não aplica ainda no banco (apenas cria os arquivos)

Mova a pasta Migrations criada pelo comando para a pasta **Infrastructure**, para seguir o padrão da divisão em camadas


#### **4. Aplicar Migração no Banco**

Ainda no PowerShell do desenvolvedor, digite o comando:
```bash
# Aplicar migrações no banco de dados
dotnet ef database update
```

**O que acontece:**
- ✅ Cria a tabela `Todo` no banco `TodoDB`
- ✅ Aplica a estrutura definida no `TodoDbContext`
- ✅ Banco fica pronto para uso

#### **5. Verificar no pgAdmin**

1. **Expanda:** TodoAPI Local → Databases → TodoDB → Schemas → public → Tables
2. **Você deve ver:** A tabela `todo` criada e suas colunas/atributos

### 🔄 CURIOSIDADE - Comandos de Migrations úteis

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




### 7. Executar a API
1. Com o **pgAdmin4** aberto, vá até sua solução (**Solução 'TodoAPI'**) no Visual Studio.  
2. No menu de execução, mude a opção **https** para **http** clicando na seta pra baixo:  

   <img width="195" height="129" alt="Selecionar http" src="https://github.com/user-attachments/assets/02298aa0-cfa8-424d-9502-0c2bb70f4379" />  

3. Para executar a API, clique no botão **Executar** (seta verde):  

   <img width="195" height="129" alt="Botão executar" src="https://github.com/user-attachments/assets/4774dfa9-92f6-4cc6-87e0-57ba5e625df7" />  


A API será iniciada e estará disponível em uma URL similar a:
- **HTTP**: `http://localhost:5000`

Para testar as requisições da API e a conexão com o banco, acesse o swagger em:
- **Swagger UI**: `https://localhost:5001/swagger` (interface para testar a API)


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

