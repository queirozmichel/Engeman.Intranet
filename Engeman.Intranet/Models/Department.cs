using System;

namespace Engeman.Intranet.Models
{
  public class Department
  {
    public int Id { get; set; }
    public string Code { get; set; }
    public string Descrption { get; set; }
    public char Active { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
