using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TodoApi.Models;

public class Comment {
    public long Id { get; set; }

    [Required]
    public string? Text { get; set; }

    public long TodoItemId { get; set; }

    [JsonIgnore] // Prevent serialization issues
    public TodoItem? TodoItem { get; set; }
}
