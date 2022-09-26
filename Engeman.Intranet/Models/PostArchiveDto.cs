using System.Collections.Generic;

namespace Engeman.Intranet.Models
{
  public class PostArchiveDto
  {
    public Post Post { get; set; }
    public PostFile Archive { get; set; }
    public List<int> DepartmentsList { get; set; }
  }
}
