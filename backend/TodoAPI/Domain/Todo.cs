namespace TodoAPI.Domain
{
    // Representa uma tarefa na aplicação
    public class Todo
    {
        // ID único da tarefa
        public Guid Id { get; set; } = Guid.NewGuid();
        
        // Descrição da tarefa
        public string Descricao { get; set; } = string.Empty;
        
        // Se a tarefa está completa ou não
        public bool Completo { get; set; } = false;
    }
}
