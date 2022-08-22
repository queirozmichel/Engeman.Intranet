using System;

namespace Engeman.Intranet.Models
{
  public class PostComment
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public int UserAccountId { get; set; }
    public int DepartmentId { get; set; }
    public int PostId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
