using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetAllAsync();
        Task<Todo?> GetByIdAsync(Guid id);
        Task<Todo> CreateAsync(string descricao, bool completo = false);
        Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo);
        Task<bool> ToggleCompleteAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);

    }
}
