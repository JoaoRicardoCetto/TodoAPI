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
            return await _context.Todo.ToListAsync();
        }

        // Buscar uma tarefa pelo ID
        public async Task<Todo?> GetByIdAsync(Guid id)
        {
            return await _context.Todo.FindAsync(id);
        }

        // Adicionar uma nova tarefa
        public async Task CreateAsync(Todo entity)
        {
            _context.Todo.Add(entity);
            await _context.SaveChangesAsync();
        }

        // Atualizar uma tarefa existente
        public async Task UpdateAsync(Todo entity)
        {
            _context.Todo.Update(entity);
            await _context.SaveChangesAsync();
        }

        // Remover uma tarefa
        public async Task DeleteAsync(Guid id)
        {
            var todo = await _context.Todo.FindAsync(id);
            if (todo != null)
            {
                _context.Todo.Remove(todo);
                await _context.SaveChangesAsync();
            }
        }
    }
}
