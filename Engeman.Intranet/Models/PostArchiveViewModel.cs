using System.Collections.Generic;

namespace Engeman.Intranet.Models
{
  public class PostArchiveViewModel
  {
    public PostArchiveViewModel()
    {
      this.Archive = new List<Archive>();
    }
    public Post Post { get; set; }
    public List<Archive> Archive { get; set; }
  }
}
