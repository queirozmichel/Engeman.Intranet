using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace Engeman.Intranet.Models.ViewModels
{
  public class NewPostViewModel
  {
    public bool Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public int UserAccountId { get; set; }
    public int DepartmentId { get; set; }
    public List<int> DepartmentsList { get; set; }
    public char PostType { get; set; }
    public bool Revised { get; set; }
  }
}
