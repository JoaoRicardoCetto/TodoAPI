using System.Collections.Concurrent;
using TodoAPI.Model;

namespace TodoAPI.Repository
{
    public class TodoRepository : IBaseRepository<Todo>
    {
        private readonly ConcurrentDictionary<Guid, Todo> _store = new();

        public Task<IEnumerable<Todo>> GetAllAsync()
            => Task.FromResult(_store.Values.AsEnumerable());

        public Task<Todo?> GetByIdAsync(Guid id)
            => Task.FromResult(_store.TryGetValue(id, out var todo) ? todo : null);

        public Task CreateAsync(Todo entity)
        {
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Todo entity)
        {
            _store[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }

    }
}