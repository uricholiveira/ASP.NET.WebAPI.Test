using Business.Interfaces;
using Data.Entities;
using Data.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly ITodoService _todoService;

    public TodoController(ILogger<TodoController> logger, ITodoService todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var todo = await _todoService.GetTodo(id, cancellationToken);
        return todo is null ? NotFound(null) : Ok(todo);
    }

    [HttpGet]
    [Authorize]
    public async Task<List<Todo>> GetAll([FromQuery] GetTodoRequestModel model, CancellationToken cancellationToken)
    {
        var todos = await _todoService.GetTodos(model, cancellationToken);
        return todos;
    }

    [HttpPost]
    public async Task<Todo> Post([FromBody] CreateTodoRequestModel model, CancellationToken cancellationToken)
    {
        var todo = await _todoService.CreateTodo(model, cancellationToken);
        return todo;
    }
}