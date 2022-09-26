using System.Collections.Generic;

namespace Engeman.Intranet.Models
{
  public class PostFileViewModel
  {
    public PostFileViewModel()
    {
      this.Files = new List<PostFile>();
    }
    public Post Post { get; set; }
    public List<PostFile> Files { get; set; }
    public List<int> DepartmentsList { get; set; }
  }
}
