using System;

namespace Engeman.Intranet.Models
{
  public class Post
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public bool Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public char PostType { get; set; }
    public bool Revised { get; set; }
    public int UserAccountId { get; set; }
    public DateTime ChangeDate { get; set; }    
  }
}