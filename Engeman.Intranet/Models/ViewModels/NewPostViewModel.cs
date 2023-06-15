using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Models
{
  public class NewPostViewModel
  {
    public NewPostViewModel()
    {
      Files = new List<NewPostFileViewModel>();
    }
    public bool Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public List<Keyword> KeywordsList { get; set; }
    public int UserAccountId { get; set; }
    public char PostType { get; set; }
    public bool Revised { get; set; }
    public List<int> DepartmentsList { get; set; }
    public List<NewPostFileViewModel> Files { get; set; }
  }
}
