using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
  public class CommentEditViewModel
  {
    public CommentEditViewModel()
    {
      Files = new List<CommentFile>();
    }
    public Comment Comment { get; set; }
    public List<CommentFile> Files { get; set; }
  }
}