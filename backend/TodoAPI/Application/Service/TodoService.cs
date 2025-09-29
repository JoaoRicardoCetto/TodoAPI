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