namespace Data.Models.Request;

public class TodoRequestModel
{
    public string Title { get; set; }
}

public class CreateTodoRequestModel : TodoRequestModel
{
    public string Description { get; set; }
}

public class GetTodoRequestModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}