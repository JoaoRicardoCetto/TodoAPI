using TodoAPI.Application.Repository;
using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    public class TodoService : ITodoService
    {
        private readonly IBaseRepository<Todo> _repo;

        public TodoService(IBaseRepository<Todo> repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Todo>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Todo?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task<Todo> CreateAsync(string descricao, bool completo = false)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição obrigatória", nameof(descricao));

            var todo = new Todo
            {
                Descricao = descricao.Trim(),
                Completo = completo
            };

            await _repo.CreateAsync(todo);
            return todo;
        }

        public async Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            if (descricao != null)
            {
                if (string.IsNullOrWhiteSpace(descricao))
                    throw new ArgumentException("Descrição não pode ficar vazia", nameof(descricao));
                existing.Descricao = descricao.Trim();
            }

            if (completo.HasValue)
                existing.Completo = completo.Value;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> ToggleCompleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Completo = !existing.Completo;
            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}