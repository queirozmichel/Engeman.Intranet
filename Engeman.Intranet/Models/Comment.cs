namespace Engeman.Intranet.Models
{
  public class Comment
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public bool Revised { get; set; }
    public int UserAccountId { get; set; }
    public int PostId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
