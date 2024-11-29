using TodoApi.Models;

namespace TodoApi.Dtos;
public class CommentDto {
  public CommentDto(Comment comment) {
    Id = comment.Id;

    if (comment.Text == null) {
      Text = "";
    } else {
      Text = comment.Text;
    }
  }

  public long Id { get; set; }
  public string Text { get; set; }

}