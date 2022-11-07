using System.Collections.Generic;

namespace Engeman.Intranet.Models.ViewModels
{
  public class EditedPostViewModel
  {
    public int Id { get; set; }
    public bool Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public List<int> DepartmentsList { get; set; }
    public bool Revised { get; set; }
  }
}
