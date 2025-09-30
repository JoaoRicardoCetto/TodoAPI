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
