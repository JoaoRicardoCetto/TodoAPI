namespace TodoAPI.Domain
{
    // Classe que representa uma entidade Todo no domínio da aplicação.
    // Esta é a classe principal que modela um item de tarefa a ser realizada.
    
    // Conceitos de C# aplicados aqui:
    // - Propriedades automáticas (auto-implemented properties)
    // - Inicialização de propriedades com valores padrão
    // - Guid como tipo de identificador único
    // - Encapsulamento através de propriedades públicas
    public class Todo
    {
        // Identificador único da tarefa.
        // Guid é um tipo de dados que gera um identificador único globalmente.
        // O valor padrão Guid.NewGuid() cria um novo GUID automaticamente.
        public Guid Id { get; set; } = Guid.NewGuid();
        
        // Descrição da tarefa a ser realizada.
        // string.Empty é uma forma mais eficiente de inicializar strings vazias
        // em comparação com "" (string vazia literal).
        public string Descricao { get; set; } = string.Empty;
        
        // Indica se a tarefa foi concluída ou não.
        // bool é um tipo de dados que pode ter apenas dois valores: true ou false.
        // O valor padrão false indica que a tarefa começa como não concluída.
        public bool Completo { get; set; } = false;
    }
}
