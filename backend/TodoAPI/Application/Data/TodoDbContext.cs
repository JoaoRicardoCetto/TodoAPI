using Microsoft.EntityFrameworkCore;
using TodoAPI.Domain;

namespace TodoAPI.Application.Data
{
    // Contexto do banco de dados para gerenciar as tarefas
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        // Define a tabela de tarefas no banco
        public DbSet<Todo> Todos { get; set; }

        // Configuração do modelo de dados
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar a entidade Todo
            modelBuilder.Entity<Todo>(entity =>
            {
                // Definir a chave primária
                entity.HasKey(t => t.Id);

                // Configurar a propriedade Descricao
                entity.Property(t => t.Descricao)
                      .IsRequired()
                      .HasMaxLength(500);

                // Configurar a propriedade Completo
                entity.Property(t => t.Completo)
                      .IsRequired()
                      .HasDefaultValue(false);

                // Definir o nome da tabela
                entity.ToTable("Todos");
            });
        }
    }
}
