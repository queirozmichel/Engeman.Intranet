namespace Engeman.Intranet.Models
{
  public class Post
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public char Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public int UserAccountId { get; set; }
  }
}
