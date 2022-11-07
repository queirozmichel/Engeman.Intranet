using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
  public class NewPostWithFilesViewModel
  {
    public NewPostWithFilesViewModel()
    {
      Post = new NewPostViewModel();
      Files = new List<NewPostFileViewModel>();
      Post.PostType = 'F';
    }
    public NewPostViewModel Post { get; set; }
    public List<NewPostFileViewModel> Files { get; set; }
    public List<int> DepartmentsList { get; set; }
  }
}