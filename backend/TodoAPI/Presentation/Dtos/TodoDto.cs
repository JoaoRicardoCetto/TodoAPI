namespace TodoAPI.Controller.Dtos
{
    // Data Transfer Object (DTO) unificado para operações com tarefas.
    // Este DTO é usado tanto para criação quanto para atualização de tarefas.
    // A flexibilidade é alcançada através do uso de tipos nullable.
     
    // Conceitos de C# e .NET aplicados aqui:
    // - DTO Pattern: separa dados de transferência da lógica de domínio
    // - Nullable reference types: string? permite valores nulos
    // - Nullable value types: bool? permite valores nulos
    // - DTO unificado: um único objeto para múltiplas operações
    // - Flexibilidade: permite operações de criação e atualização

    // Vantagens do DTO:
    // - Simplifica a manutenção
    // - Permite atualização parcial: cliente pode enviar apenas os campos que deseja alterar
    // - Para criação: Descricao é obrigatória, Completo é opcional (padrão: false)
    // - Para atualização: todos os campos são opcionais (nullable)
    public class TodoDto
    {
        // Descrição da tarefa.
        // Para criação: deve ser fornecida (não null)
        // Para atualização: string? permite que o cliente não especifique (não altera o valor atual).
        // Nullable reference type indica que o campo é opcional para atualização.
        public string? Descricao { get; set; }
        
        // Status de conclusão da tarefa.
        // Para criação: bool? permite que o cliente não especifique (será false por padrão).
        // Para atualização: bool? permite que o cliente não especifique (não altera o valor atual).
        // Nullable value type permite três estados: true, false, ou null (não especificado).
        public bool? Completo { get; set; }
    }
}
