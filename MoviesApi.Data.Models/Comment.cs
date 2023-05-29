namespace MoviesApi.Data.Models;

public class Comment
{
    public int CommentId { get; set; }
    public string UserId { get; set; }
    public string Text { get; set; }
    public int MovieId { get; set; }
    public string Username { get; set; }
}