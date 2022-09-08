using System;
using System.Collections.Generic;

namespace Engeman.Intranet.Models
{
  public class PostCommentViewModel
  {
    public PostCommentViewModel()
    {
      this.Files = new List<PostCommentFile>();
    }
    public int Id { get; set; }
    public string Description { get; set; }
    public string  Username { get; set; }
    public int UserId { get; set; }
    public string DepartmentName { get; set; }
    public List<PostCommentFile> Files { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
