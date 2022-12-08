using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
  public class NewCommentViewModel
  {
    public NewCommentViewModel()
    {
      Files = new List<NewCommentFileViewModel>();
    }
    public string Description { get; set; }
    public bool Revised { get; set; }
    public int UserAccountId { get; set; }
    public int PostId { get; set; }
    public List<NewCommentFileViewModel> Files { get; set; }
  }
}
