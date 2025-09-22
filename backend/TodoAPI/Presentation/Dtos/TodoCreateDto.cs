namespace TodoAPI.Controller.Dtos
{
    public class TodoCreateDto
    {
        public string Descricao { get; set; } = string.Empty;
        public bool? Completo { get; set; }

    }
}
