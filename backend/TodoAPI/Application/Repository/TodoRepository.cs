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