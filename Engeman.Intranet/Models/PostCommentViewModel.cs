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
    public string Description { get; set; }
    public string  Username { get; set; }
    public string DepartmentName { get; set; }
    public List<PostCommentFile> Files { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
