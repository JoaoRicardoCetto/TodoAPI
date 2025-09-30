using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.Service;
using TodoAPI.Controller.Dtos;

namespace TodoAPI.Controller.Controllers
{
    // Controller responsável por gerenciar as operações HTTP da API de tarefas.
    // Este controller implementa o padrão REST e atua como a camada de apresentação da aplicação, recebendo requisições HTTP e retornando respostas apropriadas.
     
    // Conceitos de C# e .NET aplicados aqui:
    // - Attributes: [ApiController], [Route], [HttpGet], [HttpPost], etc.
    // - Dependency Injection: recebe dependências via construtor
    // - async/await: operações assíncronas para melhor performance
    // - IActionResult: interface para retornos de ações HTTP
    // - Exception Handling: try/catch para tratamento de erros
    // - HTTP Status Codes: Ok(), NotFound(), BadRequest(), etc.
    // - Nullable operators: ?? para valores padrão
    
    [ApiController] // Marca a classe como um controller de API
    [Route("api/[controller]")] // Define a rota base como "api/Todo"
    public class TodoController : ControllerBase
    {
        // Serviço de negócio para operações com tarefas.
        // readonly garante que a referência não pode ser alterada após inicialização.
        private readonly ITodoService _service;

        // Constructor Injection é o padrão recomendado para injeção de dependência.
        // Serviço de negócio para operações com tarefas
        public TodoController(ITodoService service)
        {
            _service = service;
        }

        // Endpoint GET para recuperar todas as tarefas.
        // Rota: GET /api/Todo
        // Retorna uma lista de todas as tarefas com status 200 (OK)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list); // Retorna status 200 com a lista de tarefas
        }

        // Endpoint GET para recuperar uma tarefa específica pelo ID.
        // Rota: GET /api/Todo/{id}
        // {id:guid} garante que o parâmetro seja um GUID válido.
        // Retorna tarefa encontrada (200) ou não encontrada (404)</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var todo = await _service.GetByIdAsync(id);
            return todo is null ? NotFound() : Ok(todo);
        }

        // Endpoint POST para criar uma nova tarefa.
        // Rota: POST /api/Todo
        // [FromBody] indica que os dados vêm do corpo da requisição (JSON).
        // Parâmetro dto: Dados da tarefa a ser criada
        // Retorna tarefa criada (201) ou erro de validação (400)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoDto dto)
        {
            try
            {
                // Validação: para criação, a descrição é obrigatória
                if (string.IsNullOrWhiteSpace(dto.Descricao))
                    return BadRequest(new { message = "Descrição é obrigatória" });
                
                // Cria a tarefa usando o serviço
                var todo = await _service.CreateAsync(dto.Descricao, dto.Completo ?? false);
                
                // Retorna status 201 (Created) com a tarefa criada
                // CreatedAtAction inclui a URL da tarefa criada no cabeçalho Location
                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (ArgumentException ex)
            {
                // Retorna status 400 (Bad Request) com a mensagem de erro
                return BadRequest(new { message = ex.Message });
            }
        }

        // Endpoint PUT para atualizar uma tarefa existente.
        // Rota: PUT /api/Todo/{id}
        // PUT é usado para atualização completa ou parcial.
        // Parâmetro id: ID da tarefa a ser atualizada
        // Parâmetro dto: Dados atualizados da tarefa
        // Retorna Sucesso (204) ou não encontrada (404) ou erro de validação (400)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto.Descricao, dto.Completo);
                return ok ? NoContent() : NotFound(); // 204 (No Content) ou 404 (Not Found)
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 (Bad Request)
            }
        }

        // Endpoint PATCH para alternar o status de conclusão de uma tarefa.
        // Rota: PATCH /api/Todo/{id}/toggle
        // PATCH é usado para operações específicas/parciais.
        // Parâmetro id: ID da tarefa a ter o status alternado
        // Retorna Sucesso (200) ou não encontrada (404)
        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var ok = await _service.ToggleCompleteAsync(id);
            return ok ? Ok() : NotFound(); // 200 (OK) ou 404 (Not Found)
        }

        // Endpoint DELETE para remover uma tarefa.
        // Rota: DELETE /api/Todo/{id}
        // Parâmetro id: ID da tarefa a ser removida
        // Retorna Sucesso (204) ou não encontrada (404)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound(); // 204 (No Content) ou 404 (Not Found)
        }
    }
}