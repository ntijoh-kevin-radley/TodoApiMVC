using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
    if (!context.TodoItems.Any()) {
        context.TodoItems.Add(new TodoItem {
            Name = "Sample Todo",
            IsComplete = false,
            Comments = new List<Comment> {
                new Comment { Text = "Sample Comment 1" },
                new Comment { Text = "Sample Comment 2" }
            }
        });
        context.SaveChanges();
    }
}

app.Run();