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
