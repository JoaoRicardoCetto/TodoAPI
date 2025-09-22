namespace TodoAPI.Domain
{
    public class Todo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Descricao { get; set; } = string.Empty;
        public bool Completo { get; set; } = false;

    }
}
