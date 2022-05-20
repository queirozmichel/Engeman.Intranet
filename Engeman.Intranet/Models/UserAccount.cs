using System;

namespace Engeman.Intranet.Models
{
  public class UserAccount
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public string Name { get; set; }
    public string DomainAccount { get; set; }
    public int DepartmentId { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
