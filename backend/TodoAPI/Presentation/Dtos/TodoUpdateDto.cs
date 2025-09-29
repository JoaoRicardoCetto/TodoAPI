namespace TodoAPI.Controller.Dtos
{
    // Data Transfer Object (DTO) para atualização de tarefas.
    // Este DTO é usado para operações de atualização parcial (PATCH) ou completa (PUT).
     
    // Conceitos de C# e .NET aplicados aqui:
    // - DTO Pattern: separa dados de transferência da lógica de domínio
    // - Nullable reference types: string? permite valores nulos
    // - Nullable value types: bool? permite valores nulos
    // - Atualização parcial: apenas campos fornecidos são atualizados

    // Diferenças entre Create e Update DTOs:
    // - Create: campos obrigatórios para criação
    // - Update: todos os campos são opcionais (nullable)
    // - Permite atualização parcial: cliente pode enviar apenas os campos que deseja alterar
    public class TodoUpdateDto
    {
        // Nova descrição da tarefa.
        // string? permite que o cliente não especifique (não altera o valor atual).
        // Nullable reference type indica que o campo é opcional.
        public string? Descricao { get; set; }
        
        // Novo status de conclusão da tarefa.
        // bool? permite que o cliente não especifique (não altera o valor atual).
        // Nullable value type permite três estados: true, false, ou null (não especificado).
        public bool? Completo { get; set; }
    }
}
