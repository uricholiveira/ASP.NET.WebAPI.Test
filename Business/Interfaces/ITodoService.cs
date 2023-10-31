using Data.Entities;
using Data.Models.Request;

namespace Business.Interfaces;

public interface ITodoService
{
    public Task<Todo?> GetTodo(Guid todoId, CancellationToken cancellationToken);
    public Task<List<Todo>> GetTodos(GetTodoRequestModel model, CancellationToken cancellationToken);
    public Task<Todo> CreateTodo(CreateTodoRequestModel model, CancellationToken cancellationToken);
}