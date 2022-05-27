using System;

namespace Engeman.Intranet.Models
{
  public class UserProfile
  {
    public UserProfile()
    {
      this.Department = new Department();
    }
    public int Id { get; set; }
    public char Active { get; set; }
    public string Name { get; set; }
    public string DomainAccount { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public string Email { get; set; }
    public byte[] Photo { get; set; }
    public string Description { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
