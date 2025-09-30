namespace TodoAPI.Infrastructure.Repository
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

