using Microsoft.AspNetCore.Mvc;
using TodoAPI.Application.Service;
using TodoAPI.Controller.Dtos;

namespace TodoAPI.Controller.Controllers
{
    // Gerencia as operações da API REST para tarefas
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        // Serviço para gerenciar tarefas
        private readonly ITodoService _service;

        public TodoController(ITodoService service)
        {
            _service = service;
        }

        // Buscar todas as tarefas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // Buscar uma tarefa pelo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var todo = await _service.GetByIdAsync(id);
            return todo is null ? NotFound() : Ok(todo);
        }

        // Criar uma nova tarefa
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TodoDto dto)
        {
            try
            {
                // Verificar se a descrição foi fornecida
                if (string.IsNullOrWhiteSpace(dto.Descricao))
                    return BadRequest(new { message = "Descrição é obrigatória" });
                
                // Criar a tarefa
                var todo = await _service.CreateAsync(dto.Descricao, dto.Completo ?? false);
                
                // Retornar a tarefa criada
                return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Atualizar uma tarefa existente
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TodoDto dto)
        {
            try
            {
                var ok = await _service.UpdateAsync(id, dto.Descricao, dto.Completo);
                return ok ? NoContent() : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Alternar o status de uma tarefa
        [HttpPatch("{id:guid}/toggle")]
        public async Task<IActionResult> Toggle(Guid id)
        {
            var ok = await _service.ToggleCompleteAsync(id);
            return ok ? Ok() : NotFound();
        }

        // Remover uma tarefa
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}