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