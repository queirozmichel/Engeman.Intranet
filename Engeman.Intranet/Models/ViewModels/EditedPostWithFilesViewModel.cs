using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
  public class EditedPostWithFilesViewModel
  {
    public EditedPostWithFilesViewModel()
    {
      Files = new List<EditedPostFileViewModel>();
    }

    public EditedPostViewModel Post { get; set; }
    public List<EditedPostFileViewModel> Files { get; set; }
  }
}
