namespace TodoAPI.Controller.Dtos
{
    // Data Transfer Object (DTO) para criação de tarefas.
    // DTOs são objetos simples usados para transferir dados entre camadas da aplicação, especialmente entre a API e o cliente.

    // Conceitos de C# e .NET aplicados aqui:
    // - DTO Pattern: separa dados de transferência da lógica de domínio
    // - Propriedades automáticas: get/set simplificados
    // - Inicialização de propriedades: valores padrão
    // - Nullable value types: bool? permite valores nulos
    // - string.Empty: forma eficiente de inicializar strings vazias
     
    // Vantagens dos DTOs:
    // - Controle sobre quais dados são expostos pela API
    // - Validação específica para entrada de dados
    // - Desacoplamento entre camadas
    // - Segurança: evita exposição de propriedades internas
    public class TodoCreateDto
    {
        // Descrição da tarefa a ser criada.
        // Propriedade obrigatória para criação de tarefas.
        public string Descricao { get; set; } = string.Empty;
        
        // Status inicial de conclusão da tarefa.
        // bool? permite que o cliente não especifique o valor (será false por padrão).
        // Nullable value type permite três estados: true, false, ou null (não especificado).
        public bool? Completo { get; set; }
    }
}
