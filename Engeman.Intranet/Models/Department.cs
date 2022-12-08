using System;

namespace Engeman.Intranet.Models
{
  public class Department
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}