using System.Collections.Generic;

namespace Engeman.Intranet.Models
{
  public class CommentEditViewModel
  {
    public CommentEditViewModel()
    {
      Files = new List<PostCommentFile>();
    }
    public PostComment Comment { get; set; }
    public List<PostCommentFile> Files { get; set; }
  }
}