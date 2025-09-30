# 🚀 TodoAPI - Tutorial de Desenvolvimento Backend

Este tutorial demonstra como desenvolver uma API REST completa para gerenciamento de tarefas (Todo) usando **.NET 8** e **ASP.NET Core**. A aplicação segue os princípios de **Clean Architecture** e **SOLID**.

## 📋 Índice

- [Visão Geral da Arquitetura](#-visão-geral-da-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Camadas da Aplicação](#-camadas-da-aplicação)
- [Implementação Passo a Passo](#-implementação-passo-a-passo)
- [Configuração e Execução](#-configuração-e-execução)
- [Endpoints da API](#-endpoints-da-api)
- [Conceitos Aplicados](#-conceitos-aplicados)

## 🏗️ Visão Geral da Arquitetura

A aplicação segue o padrão **Clean Architecture** com três camadas principais:

```
📁 TodoAPI/
├── 🎯 Domain/          # Entidades e regras de negócio
├── 🔧 Application/     # Lógica de aplicação (Services + Repositories)
└── 🌐 Presentation/    # Controllers e DTOs
```

### Padrões Utilizados:
- **Repository Pattern**: Abstração do acesso a dados
- **Service Layer**: Lógica de negócio
- **DTO Pattern**: Transferência de dados
- **Dependency Injection**: Inversão de controle
- **REST API**: Endpoints padronizados

## 📂 Estrutura do Projeto

```
TodoAPI/
├── Domain/
│   └── Todo.cs                    # Entidade principal
├── Application/
│   ├── Repository/
│   │   ├── IBaseRepository.cs     # Interface genérica do repositório
│   │   └── TodoRepository.cs      # Implementação em memória
│   └── Service/
│       ├── ITodoService.cs        # Interface do serviço
│       └── TodoService.cs         # Lógica de negócio
├── Presentation/
│   ├── Controllers/
│   │   └── TodoController.cs      # Controller REST
│   └── Dtos/
│       └── TodoDto.cs             # DTO unificado para criação e atualização
├── Program.cs                     # Configuração da aplicação
└── TodoAPI.csproj                # Arquivo do projeto
```

## 🎯 Camadas da Aplicação

### 1. **Domain Layer** (Camada de Domínio)
Responsável pelas **entidades** e **regras de negócio** fundamentais.

### 2. **Application Layer** (Camada de Aplicação)
Contém a **lógica de negócio** e **abstrações** para acesso a dados.

### 3. **Presentation Layer** (Camada de Apresentação)
Gerencia as **requisições HTTP** e **respostas** da API.

## 🛠️ Implementação Passo a Passo

### Passo 1: Configuração do Projeto

Crie um novo projeto ASP.NET Core Web API:

```bash
dotnet new webapi -n TodoAPI
cd TodoAPI
```

### Passo 2: Entidade de Domínio

Crie a entidade `Todo` na pasta `Domain/`:

```csharp
namespace TodoAPI.Domain
{
    // Classe que representa uma entidade Todo no domínio da aplicação.
    // Esta é a classe principal que modela um item de tarefa a ser realizada.
    
    // Conceitos de C# aplicados aqui:
    // - Propriedades automáticas (auto-implemented properties)
    // - Inicialização de propriedades com valores padrão
    // - Guid como tipo de identificador único
    // - Encapsulamento através de propriedades públicas
    public class Todo
    {
        // Identificador único da tarefa.
        // Guid é um tipo de dados que gera um identificador único globalmente.
        // O valor padrão Guid.NewGuid() cria um novo GUID automaticamente.
        public Guid Id { get; set; } = Guid.NewGuid();
        
        // Descrição da tarefa a ser realizada.
        // string.Empty é uma forma mais eficiente de inicializar strings vazias
        // em comparação com "" (string vazia literal).
        public string Descricao { get; set; } = string.Empty;
        
        // Indica se a tarefa foi concluída ou não.
        // bool é um tipo de dados que pode ter apenas dois valores: true ou false.
        // O valor padrão false indica que a tarefa começa como não concluída.
        public bool Completo { get; set; } = false;
    }
}
```

**🎯 Responsabilidade:** Representa a entidade principal do sistema com suas propriedades e comportamentos básicos.

### Passo 3: Interface do Repositório

Crie a interface genérica `IBaseRepository` na pasta `Application/Repository/`:

```csharp
namespace TodoAPI.Application.Repository
{   
    // Interface genérica que define operações básicas de repositório para qualquer entidade.
    
    // Conceitos de C# e .NET aplicados aqui:
    // - Interfaces: contrato que define o que uma classe deve implementar
    // - Generics (<T>): permite que a interface trabalhe com qualquer tipo de entidade
    // - Async/Await: operações assíncronas para melhor performance
    // - Task: representa uma operação assíncrona
    // - IEnumerable: interface para coleções que podem ser enumeradas
    // - Nullable reference types (T?): permite valores nulos
    
    // Padrão Repository: abstrai a lógica de acesso a dados
    // T = Tipo da entidade que o repositório irá gerenciar
    public interface IBaseRepository<T>
    {
        // Recupera todas as entidades do repositório.
        // Task<IEnumerable<T>> indica que retorna uma tarefa assíncrona que, quando completada,
        // retornará uma coleção enumerável de entidades do tipo T.
        // Retorna uma tarefa assíncrona que retorna uma coleção de entidades
        Task<IEnumerable<T>> GetAllAsync();
        
        // Recupera uma entidade específica pelo seu ID.
        // T? indica que o retorno pode ser null (nullable reference type).
        // Parâmetro id: Identificador único da entidade
        // Retorna uma tarefa assíncrona que retorna a entidade ou null se não encontrada
        Task<T?> GetByIdAsync(Guid id);
        
        // Cria uma nova entidade no repositório.
        // Parâmetro entity: Entidade a ser criada
        // Retorna uma tarefa assíncrona que representa a operação de criação
        Task CreateAsync(T entity);
        
        // Atualiza uma entidade existente no repositório.
        // Parâmetro entity: Entidade com os dados atualizados
        // Retorna uma tarefa assíncrona que representa a operação de atualização
        Task UpdateAsync(T entity);
        
        // Remove uma entidade do repositório pelo seu ID.
        // Parâmetro id: Identificador único da entidade a ser removida
        // Retorna uma tarefa assíncrona que representa a operação de remoção
        Task DeleteAsync(Guid id);
    }
}
```

**🎯 Responsabilidade:** Define o contrato para operações de acesso a dados, permitindo diferentes implementações (memória, banco de dados, etc.).

### Passo 4: Implementação do Repositório

Implemente o `TodoRepository` na pasta `Application/Repository/`:

```csharp
using System.Collections.Concurrent;
using TodoAPI.Domain;

namespace TodoAPI.Application.Repository
{
    // Implementação concreta do repositório para a entidade Todo.
    // Esta classe implementa a interface IBaseRepository<Todo> e fornece
    // uma implementação em memória usando ConcurrentDictionary.
    
    // Conceitos de C# e .NET aplicados aqui:
    // - Herança de interface: implementa IBaseRepository<Todo>
    // - ConcurrentDictionary: coleção thread-safe para operações concorrentes
    // - readonly: campo que só pode ser atribuído na declaração ou no construtor
    // - Expression-bodied members: métodos que retornam uma expressão simples
    // - Task.FromResult: cria uma tarefa já completada com um resultado
    // - Task.CompletedTask: representa uma tarefa já completada sem resultado
    // - Pattern matching: uso de 'out var' para capturar valores
    // - Operador ternário: condição ? valorSeVerdadeiro : valorSeFalso
    public class TodoRepository : IBaseRepository<Todo>
    {
        // Armazenamento em memória usando ConcurrentDictionary.
        // ConcurrentDictionary é thread-safe, permitindo operações concorrentes
        // sem necessidade de locks manuais.
        // readonly garante que a referência não pode ser alterada após a inicialização.
        private readonly ConcurrentDictionary<Guid, Todo> _store = new();

        // Recupera todas as tarefas armazenadas.
        // _store.Values retorna uma coleção de todos os valores (todos) no dicionário.
        // AsEnumerable() converte para IEnumerable<Todo>.
        // Task.FromResult() cria uma tarefa já completada com o resultado.
        // Retorna uma tarefa assíncrona que retorna todas as tarefas
        public Task<IEnumerable<Todo>> GetAllAsync()
            => Task.FromResult(_store.Values.AsEnumerable());

        // Recupera uma tarefa específica pelo ID.
        // TryGetValue tenta obter o valor do dicionário e retorna true se encontrado.
        // O operador ternário retorna o todo se encontrado, ou null se não encontrado.
        // Parâmetro id: ID da tarefa a ser recuperada
        // Retorna uma tarefa assíncrona que retorna a tarefa ou null
        public Task<Todo?> GetByIdAsync(Guid id)
            => Task.FromResult(_store.TryGetValue(id, out var todo) ? todo : null);

        // Adiciona uma nova tarefa ao repositório.
        // A indexação do dicionário adiciona ou atualiza o valor.
        // Task.CompletedTask retorna uma tarefa já completada.
        // Parâmetro entity: Tarefa a ser adicionada
        // Retorna uma tarefa assíncrona que representa a operação
        public Task CreateAsync(Todo entity)
        {
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }

        // Atualiza uma tarefa existente no repositório.
        // Como estamos usando um dicionário, a atualização é feita da mesma forma
        // que a criação (sobrescrevendo o valor).
        // Parâmetro entity: Tarefa com os dados atualizados
        // Retorna uma tarefa assíncrona que representa a operação
        public Task UpdateAsync(Todo entity)
        {
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }

        // Remove uma tarefa do repositório.
        // TryRemove remove o item e retorna true se removido com sucesso.
        // O 'out _' descarta o valor removido (não precisamos dele).
        // Parâmetro id: ID da tarefa a ser removida
        // Retorna uma tarefa assíncrona que representa a operação
        public Task DeleteAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }
    }
}
```

**🎯 Responsabilidade:** Implementa as operações de persistência usando armazenamento em memória thread-safe.

### Passo 5: Interface do Serviço

Crie a interface `ITodoService` na pasta `Application/Service/`:

```csharp
using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    // Interface que define as operações de negócio para gerenciamento de tarefas.
    // Esta interface define a camada de serviço, que contém a lógica de negócio
    // e coordena as operações entre a apresentação e o repositório.
    
    // Conceitos de C# e .NET aplicados aqui:
    // - Interface: contrato que define operações de negócio
    // - Async/Await: todas as operações são assíncronas
    // - Parâmetros opcionais: bool completo = false
    // - Nullable reference types: string? e bool? permitem valores nulos
    // - Task<T>: retorna tarefas assíncronas com tipos específicos
    
    // Padrão Service Layer: encapsula a lógica de negócio
    public interface ITodoService
    {
        // Recupera todas as tarefas do sistema.
        // Retorna uma tarefa assíncrona que retorna uma coleção de todas as tarefas
        Task<IEnumerable<Todo>> GetAllAsync();
        
        // Recupera uma tarefa específica pelo seu ID.
        // Parâmetro id: Identificador único da tarefa
        // Retorna uma tarefa assíncrona que retorna a tarefa ou null se não encontrada
        Task<Todo?> GetByIdAsync(Guid id);
        
        // Cria uma nova tarefa no sistema.
        // Parâmetros opcionais permitem definir valores padrão.
        // Parâmetro descricao: Descrição da tarefa (obrigatória)
        // Parâmetro completo: Se a tarefa já está completa (padrão: false)
        // Retorna uma tarefa assíncrona que retorna a tarefa criada
        Task<Todo> CreateAsync(string descricao, bool completo = false);
        
        // Atualiza uma tarefa existente.
        // Parâmetros nullable permitem atualização parcial.
        // Parâmetro id: ID da tarefa a ser atualizada
        // Parâmetro descricao: Nova descrição (null se não deve ser alterada)
        // Parâmetro completo: Novo status de conclusão (null se não deve ser alterado)
        // Retorna uma tarefa assíncrona que retorna true se a atualização foi bem-sucedida
        Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo);
        
        // Alterna o status de conclusão de uma tarefa.
        // Se estiver completa, marca como incompleta e vice-versa.
        // Parâmetro id: ID da tarefa a ter o status alternado
        // Retorna uma tarefa assíncrona que retorna true se a operação foi bem-sucedida
        Task<bool> ToggleCompleteAsync(Guid id);
        
        // Remove uma tarefa do sistema.
        // Parâmetro id: ID da tarefa a ser removida
        // Retorna uma tarefa assíncrona que retorna true se a remoção foi bem-sucedida
        Task<bool> DeleteAsync(Guid id);
    }
}
```

**🎯 Responsabilidade:** Define as operações de negócio que podem ser executadas no sistema.

### Passo 6: Implementação do Serviço

Implemente o `TodoService` na pasta `Application/Service/`:

```csharp
using TodoAPI.Application.Repository;
using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    // Implementação concreta do serviço de gerenciamento de tarefas.
    // Esta classe implementa a lógica de negócio e coordena as operações
    // entre a camada de apresentação e o repositório.
    
    // Conceitos de C# e .NET aplicados aqui:
    // - Dependency Injection: recebe dependências via construtor
    // - readonly: campo que não pode ser alterado após inicialização
    // - async/await: operações assíncronas
    // - Expression-bodied members: métodos que retornam expressões simples
    // - Object initializer: sintaxe para inicializar objetos
    // - Nullable reference types: verificação de valores nulos
    // - Exception handling: lançamento de exceções para validação
    // - String manipulation: Trim(), IsNullOrWhiteSpace()
    // - Nullable value types: HasValue, Value
    public class TodoService : ITodoService
    {
        // Repositório para operações de persistência.
        // readonly garante que a referência não pode ser alterada após a inicialização.
        private readonly IBaseRepository<Todo> _repo;

        // Construtor que recebe o repositório via Dependency Injection.
        // Este é um exemplo do padrão Constructor Injection.
        // Parâmetro repo: Repositório para operações de dados
        public TodoService(IBaseRepository<Todo> repo)
        {
            _repo = repo;
        }

        // Recupera todas as tarefas.
        // Delega a operação diretamente para o repositório.
        // Retorna uma tarefa assíncrona que retorna todas as tarefas
        public Task<IEnumerable<Todo>> GetAllAsync() => _repo.GetAllAsync();

        // Recupera uma tarefa específica pelo ID.
        // Delega a operação diretamente para o repositório.
        // Parâmetro id: ID da tarefa
        // Retorna uma tarefa assíncrona que retorna a tarefa ou null
        public Task<Todo?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        // Cria uma nova tarefa com validação de negócio.
        // Aplica regras de negócio antes de persistir no repositório.
        // Parâmetro descricao: Descrição da tarefa
        // Parâmetro completo: Se a tarefa está completa (padrão: false)
        // Retorna uma tarefa assíncrona que retorna a tarefa criada
        // Exceção ArgumentException: Lançada quando a descrição é inválida
        public async Task<Todo> CreateAsync(string descricao, bool completo = false)
        {
            // Validação de negócio: descrição não pode ser vazia
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição obrigatória", nameof(descricao));

            // Criação do objeto usando object initializer
            var todo = new Todo
            {
                Descricao = descricao.Trim(), // Remove espaços em branco
                Completo = completo
            };

            // Persiste no repositório
            await _repo.CreateAsync(todo);
            return todo;
        }

        // Atualiza uma tarefa existente com validações.
        // Permite atualização parcial (apenas campos fornecidos).
        // Parâmetro id: ID da tarefa a ser atualizada
        // Parâmetro descricao: Nova descrição (null se não deve ser alterada)
        // Parâmetro completo: Novo status (null se não deve ser alterado)
        // Retorna True se a atualização foi bem-sucedida
        // Exceção ArgumentException: Lançada quando a descrição é inválida
        public async Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo)
        {
            // Verifica se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            // Atualiza descrição se fornecida
            if (descricao != null)
            {
                if (string.IsNullOrWhiteSpace(descricao))
                    throw new ArgumentException("Descrição não pode ficar vazia", nameof(descricao));
                existing.Descricao = descricao.Trim();
            }

            // Atualiza status se fornecido
            if (completo.HasValue)
                existing.Completo = completo.Value;

            // Persiste as alterações
            await _repo.UpdateAsync(existing);
            return true;
        }

        // Alterna o status de conclusão de uma tarefa.
        // Se estiver completa, marca como incompleta e vice-versa.
        // Parâmetro id: ID da tarefa
        // Retorna True se a operação foi bem-sucedida
        public async Task<bool> ToggleCompleteAsync(Guid id)
        {
            // Verifica se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            // Alterna o status usando o operador de negação (!)
            existing.Completo = !existing.Completo;
            
            // Persiste a alteração
            await _repo.UpdateAsync(existing);
            return true;
        }

        // Remove uma tarefa do sistema.
        // Verifica se a tarefa existe antes de remover.
        // Parâmetro id: ID da tarefa a ser removida
        // Retorna True se a remoção foi bem-sucedida
        public async Task<bool> DeleteAsync(Guid id)
        {
            // Verifica se a tarefa existe
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            
            // Remove do repositório
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
```

**🎯 Responsabilidade:** Implementa a lógica de negócio, validações e coordena as operações entre controller e repositório.

### Passo 7: DTO Unificado (Data Transfer Object)

Crie o DTO unificado na pasta `Presentation/Dtos/`:

#### TodoDto.cs
```csharp
namespace TodoAPI.Controller.Dtos
{
    // Data Transfer Object (DTO) unificado para operações com tarefas.
    // Este DTO é usado tanto para criação quanto para atualização de tarefas.
    // A flexibilidade é alcançada através do uso de tipos nullable.
     
    // Conceitos de C# e .NET aplicados aqui:
    // - DTO Pattern: separa dados de transferência da lógica de domínio
    // - Nullable reference types: string? permite valores nulos
    // - Nullable value types: bool? permite valores nulos
    // - DTO unificado: um único objeto para múltiplas operações
    // - Flexibilidade: permite operações de criação e atualização

    // Vantagens do DTO unificado:
    // - Reduz duplicação de código
    // - Simplifica a manutenção
    // - Permite atualização parcial: cliente pode enviar apenas os campos que deseja alterar
    // - Para criação: Descricao é obrigatória, Completo é opcional (padrão: false)
    // - Para atualização: todos os campos são opcionais (nullable)
    public class TodoDto
    {
        // Descrição da tarefa.
        // Para criação: deve ser fornecida (não null)
        // Para atualização: string? permite que o cliente não especifique (não altera o valor atual).
        // Nullable reference type indica que o campo é opcional para atualização.
        public string? Descricao { get; set; }
        
        // Status de conclusão da tarefa.
        // Para criação: bool? permite que o cliente não especifique (será false por padrão).
        // Para atualização: bool? permite que o cliente não especifique (não altera o valor atual).
        // Nullable value type permite três estados: true, false, ou null (não especificado).
        public bool? Completo { get; set; }
    }
}
```

**🎯 Responsabilidade:** O DTO unificado controla quais dados são expostos pela API, facilita a validação de entrada e reduz duplicação de código.

### Passo 8: Controller

Implemente o `TodoController` na pasta `Presentation/Controllers/`:

```csharp
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.Service;
using TodoAPI.Controller.Dtos;

namespace TodoAPI.Controller.Controllers
{
    // Controller responsável por gerenciar as operações HTTP da API de tarefas.
    // Este controller implementa o padrão REST e atua como a camada de apresentação da aplicação, recebendo requisições HTTP e retornando respostas apropriadas.
     
    // Conceitos de C# e .NET aplicados aqui:
    // - Attributes: [ApiController], [Route], [HttpGet], [HttpPost], etc.
    // - Dependency Injection: recebe dependências via construtor
    // - async/await: operações assíncronas para melhor performance
    // - IActionResult: interface para retornos de ações HTTP
    // - Exception Handling: try/catch para tratamento de erros
    // - HTTP Status Codes: Ok(), NotFound(), BadRequest(), etc.
    // - Nullable operators: ?? para valores padrão
    
    [ApiController] // Marca a classe como um controller de API
    [Route("api/[controller]")] // Define a rota base como "api/Todo"
    public class TodoController : ControllerBase
    {
        // Serviço de negócio para operações com tarefas.
        // readonly garante que a referência não pode ser alterada após inicialização.
        private readonly ITodoService _service;

        // Constructor Injection é o padrão recomendado para injeção de dependência.
        // Serviço de negócio para operações com tarefas
        public TodoController(ITodoService service)
        {
            _service = service;
        }

        // Endpoint GET para recuperar todas as tarefas.
        // Rota: GET /api/Todo
        // Retorna uma lista de todas as tarefas com status 200 (OK)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list); // Retorna status 200 com a lista de tarefas
        }

        // Endpoint GET para recuperar uma tarefa específica pelo ID.
        // Rota: GET /api/Todo/{id}
        // {id:guid} garante que o parâmetro seja um GUID válido.
        // Retorna tarefa encontrada (200) ou não encontrada (404)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var todo = await _service.GetByIdAsync(id);
            return todo is null ? NotFound() : Ok(todo);
        }

        // Endpoint POST para criar uma nova tarefa.
        // Rota: POST /api/Todo
        // [FromBody] indica que os dados vêm do corpo da requisição (JSON).
        // Parâmetro dto: Dados da tarefa a ser criada
        // Retorna tarefa criada (201) ou erro de validação (400)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoDto dto)
        {
            try
            {
                // Validação: para criação, a descrição é obrigatória
                if (string.IsNullOrWhiteSpace(dto.Descricao))
                    return BadRequest(new { message = "Descrição é obrigatória" });
                
                // Cria a tarefa usando o serviço
                var todo = await _service.CreateAsync(dto.Descricao, dto.Completo ?? false);
                
                // Retorna status 201 (Created) com a tarefa criada
                // CreatedAtAction inclui a URL da tarefa criada no cabeçalho Location
                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (ArgumentException ex)
            {
                // Retorna status 400 (Bad Request) com a mensagem de erro
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint PUT para atualizar uma tarefa existente.
        // Rota: PUT /api/Todo/{id}
        // PUT é usado para atualização completa ou parcial.
        // Parâmetro id: ID da tarefa a ser atualizada
        // Parâmetro dto: Dados atualizados da tarefa
        // Retorna Sucesso (204) ou não encontrada (404) ou erro de validação (400)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto.Descricao, dto.Completo);
                return ok ? NoContent() : NotFound(); // 204 (No Content) ou 404 (Not Found)
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 (Bad Request)
            }
        }

        // Endpoint PATCH para alternar o status de conclusão de uma tarefa.
        // Rota: PATCH /api/Todo/{id}/toggle
        // PATCH é usado para operações específicas/parciais.
        // Parâmetro id: ID da tarefa a ter o status alternado
        // Retorna Sucesso (200) ou não encontrada (404)
        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var ok = await _service.ToggleCompleteAsync(id);
            return ok ? Ok() : NotFound(); // 200 (OK) ou 404 (Not Found)
        }

        // Endpoint DELETE para remover uma tarefa.
        // Rota: DELETE /api/Todo/{id}
        // Parâmetro id: ID da tarefa a ser removida
        // Retorna Sucesso (204) ou não encontrada (404)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound(); // 204 (No Content) ou 404 (Not Found)
        }
    }
}
```

**🎯 Responsabilidade:** Gerencia as requisições HTTP, valida entrada, chama serviços e retorna respostas apropriadas.

### Passo 9: Configuração da Aplicação

Configure o `Program.cs`:

```csharp
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

// Configuração de Dependency Injection (Injeção de Dependência).
// Registra os serviços no container DI para serem injetados automaticamente.

// Service Lifetimes:
// - Singleton: uma única instância durante toda a vida da aplicação
// - Scoped: uma instância por requisição HTTP
// - Transient: uma nova instância a cada solicitação
builder.Services.AddSingleton<IBaseRepository<Todo>, TodoRepository>(); // Singleton para o repositório (dados em memória)
builder.Services.AddScoped<ITodoService, TodoService>(); // Scoped para o serviço (uma instância por requisição)

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
```

**🎯 Responsabilidade:** Configura a aplicação, registra serviços, define middleware e inicializa o servidor web.

## ⚙️ Configuração e Execução

### Arquivo de Projeto (TodoAPI.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UserSecretsId>b2c5dbce-7d1a-412c-9418-d10d349013f8</UserSecretsId>
    <DockerDefaultTargetOS>Linux</DockerDefaultTargetOS>
  </PropertyGroup>

  <ItemGroup>
    <Compile Remove="NovaPasta\**" />
    <Content Remove="NovaPasta\**" />
    <EmbeddedResource Remove="NovaPasta\**" />
    <None Remove="NovaPasta\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.Azure.Containers.Tools.Targets" Version="1.22.1" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="9.0.4" />
  </ItemGroup>

</Project>
```

### Comandos para Executar

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

A aplicação estará disponível em:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`

## 🔗 Endpoints da API

| Método | Endpoint | Descrição | Parâmetros |
|--------|----------|-----------|------------|
| `GET` | `/api/Todo` | Listar todas as tarefas | - |
| `GET` | `/api/Todo/{id}` | Buscar tarefa por ID | `id` (Guid) |
| `POST` | `/api/Todo` | Criar nova tarefa | Body: `TodoDto` |
| `PUT` | `/api/Todo/{id}` | Atualizar tarefa | `id` (Guid) + Body: `TodoDto` |
| `PATCH` | `/api/Todo/{id}/toggle` | Alternar status da tarefa | `id` (Guid) |
| `DELETE` | `/api/Todo/{id}` | Excluir tarefa | `id` (Guid) |

### Exemplos de Uso

#### Criar uma tarefa
```bash
# Criar com descrição obrigatória
curl -X POST "https://localhost:5001/api/Todo" \
     -H "Content-Type: application/json" \
     -d '{"descricao": "Estudar C#", "completo": false}'

# Criar apenas com descrição (completo será false por padrão)
curl -X POST "https://localhost:5001/api/Todo" \
     -H "Content-Type: application/json" \
     -d '{"descricao": "Estudar .NET"}'
```

#### Listar todas as tarefas
```bash
curl -X GET "https://localhost:5001/api/Todo"
```

#### Atualizar uma tarefa
```bash
# Atualização completa
curl -X PUT "https://localhost:5001/api/Todo/{id}" \
     -H "Content-Type: application/json" \
     -d '{"descricao": "Estudar C# e .NET", "completo": true}'

# Atualização parcial (apenas descrição)
curl -X PUT "https://localhost:5001/api/Todo/{id}" \
     -H "Content-Type: application/json" \
     -d '{"descricao": "Estudar C# Avançado"}'

# Atualização parcial (apenas status)
curl -X PUT "https://localhost:5001/api/Todo/{id}" \
     -H "Content-Type: application/json" \
     -d '{"completo": true}'
```

#### Alternar status
```bash
curl -X PATCH "https://localhost:5001/api/Todo/{id}/toggle"
```

## 🎓 Conceitos Aplicados

### **Padrões de Arquitetura**
- ✅ **Clean Architecture**: Separação clara de responsabilidades
- ✅ **Repository Pattern**: Abstração do acesso a dados
- ✅ **Service Layer**: Encapsulamento da lógica de negócio
- ✅ **DTO Pattern**: Transferência de dados entre camadas (DTO unificado)

### **Princípios SOLID**
- ✅ **S**ingle Responsibility: Cada classe tem uma responsabilidade
- ✅ **O**pen/Closed: Aberto para extensão, fechado para modificação
- ✅ **L**iskov Substitution: Interfaces bem definidas
- ✅ **I**nterface Segregation: Interfaces específicas
- ✅ **D**ependency Inversion: Dependência de abstrações

### **Conceitos de C# e .NET**
- ✅ **Async/Await**: Operações assíncronas
- ✅ **Dependency Injection**: Inversão de controle
- ✅ **Generics**: Código reutilizável
- ✅ **Nullable Reference Types**: Segurança de tipos
- ✅ **Expression-bodied Members**: Sintaxe concisa
- ✅ **Object Initializers**: Inicialização de objetos
- ✅ **ConcurrentDictionary**: Thread-safety

### **Boas Práticas**
- ✅ **Nomenclatura**: Convenções claras e descritivas
- ✅ **Comentários**: Documentação inline explicativa
- ✅ **Tratamento de Erros**: Validação e exceções apropriadas
- ✅ **HTTP Status Codes**: Respostas semânticas
- ✅ **RESTful API**: Endpoints padronizados
- ✅ **Separation of Concerns**: Responsabilidades bem definidas

## 🚀 Próximos Passos

Para evoluir esta aplicação, você pode:

1. **Persistência**: Implementar repositório com Entity Framework Core
2. **Validação**: Adicionar FluentValidation ou Data Annotations
3. **Autenticação**: Implementar JWT ou Identity
4. **Testes**: Adicionar testes unitários e de integração
5. **Logging**: Implementar Serilog
6. **Documentação**: Melhorar documentação da API
7. **Performance**: Adicionar cache e otimizações
8. **Monitoramento**: Implementar Health Checks

---

**🎉 Parabéns!** Você agora tem uma API REST completa seguindo as melhores práticas de desenvolvimento com .NET 8 e ASP.NET Core!
