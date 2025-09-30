namespace TodoAPI.Controller.Dtos
{
    // DTO para enviar dados de tarefas pela API
    // Funciona tanto para criar quanto para atualizar tarefas
    public class TodoDto
    {
        // Descrição da tarefa
        public string? Descricao { get; set; }
        
        // Se a tarefa está completa ou não
        public bool? Completo { get; set; }
    }
}
