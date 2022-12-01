using System;

namespace Engeman.Intranet.Models
{
  public class PostComment
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public int UserAccountId { get; set; }
    public int DepartmentId { get; set; }
    public int PostId { get; set; }
    public bool Revised { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
