using Business.Interfaces;
using Data.Entities;
using Data.Helpers;
using Data.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class TodoService : ITodoService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<TodoService> _logger;

    public TodoService(ILogger<TodoService> logger, DatabaseContext databaseContext)
    {
        _logger = logger;
        _databaseContext = databaseContext;
    }

    public async Task<Todo?> GetTodo(Guid todoId, CancellationToken cancellationToken)
    {
        return await _databaseContext.Todos.FirstOrDefaultAsync(x => x.Id == todoId, cancellationToken);
    }

    public async Task<List<Todo>> GetTodos(GetTodoRequestModel model, CancellationToken cancellationToken)
    {
        var query = _databaseContext.Todos.AsQueryable();

        if (model.Title is not null) query = query.Where(x => x.Title == model.Title);
        if (model.Description is not null) query = query.Where(x => x.Description == model.Description);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Todo> CreateTodo(CreateTodoRequestModel model, CancellationToken cancellationToken)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Description = model.Description
        };

        await _databaseContext.Todos.AddAsync(todo, cancellationToken);
        await _databaseContext.SaveChangesAsync(cancellationToken);

        return todo;
    }
}