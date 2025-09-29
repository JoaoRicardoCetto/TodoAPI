using TodoAPI.Domain;

namespace TodoAPI.Application.Service
{
    // Interface que define as operações de negócio para gerenciamento de tarefas.
    // Esta interface define a camada de serviço, que contém a lógica de negócio
    // e coordena as operações entre a apresentação e o repositório.
    
    // Conceitos de C# e .NET aplicados aqui:
    // - Interface: contrato que define operações de negócio
    // - Async/Await: todas as operações são assíncronas
    // - Parâmetros opcionais: bool completo = false
    // - Nullable reference types: string? e bool? permitem valores nulos
    // - Task<T>: retorna tarefas assíncronas com tipos específicos
    
    // Padrão Service Layer: encapsula a lógica de negócio
    public interface ITodoService
    {
        // Recupera todas as tarefas do sistema.
        // Retorna uma tarefa assíncrona que retorna uma coleção de todas as tarefas
        Task<IEnumerable<Todo>> GetAllAsync();
        
        // Recupera uma tarefa específica pelo seu ID.
        // Parâmetro id: Identificador único da tarefa
        // Retorna uma tarefa assíncrona que retorna a tarefa ou null se não encontrada
        Task<Todo?> GetByIdAsync(Guid id);
        
        // Cria uma nova tarefa no sistema.
        // Parâmetros opcionais permitem definir valores padrão.
        // Parâmetro descricao: Descrição da tarefa (obrigatória)
        // Parâmetro completo: Se a tarefa já está completa (padrão: false)
        // Retorna uma tarefa assíncrona que retorna a tarefa criada
        Task<Todo> CreateAsync(string descricao, bool completo = false);
        
        // Atualiza uma tarefa existente.
        // Parâmetros nullable permitem atualizar apenas campos específicos.
        // Parâmetro id: ID da tarefa a ser atualizada
        // Parâmetro descricao: Nova descrição (null se não deve ser alterada)
        // Parâmetro completo: Novo status de conclusão (null se não deve ser alterado)
        // Retorna uma tarefa assíncrona que retorna true se a atualização foi bem-sucedida
        Task<bool> UpdateAsync(Guid id, string? descricao, bool? completo);
        
        // Alterna o status de conclusão de uma tarefa.
        // Se estiver completa, marca como incompleta e vice-versa.
        // Parâmetro id: ID da tarefa a ter o status alternado
        // Retorna uma tarefa assíncrona que retorna true se a operação foi bem-sucedida
        Task<bool> ToggleCompleteAsync(Guid id);
        
        // Remove uma tarefa do sistema.
        // Parâmetro id: ID da tarefa a ser removida
        // Retorna uma tarefa assíncrona que retorna true se a remoção foi bem-sucedida
        Task<bool> DeleteAsync(Guid id);
    }
}
