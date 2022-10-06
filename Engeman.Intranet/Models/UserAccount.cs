using System;
using System.ComponentModel.DataAnnotations;

namespace Engeman.Intranet.Models
{
  public class UserAccount
  {
    public UserAccount()
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
    public bool CreatePost { get; set; }
    public bool EditOwnerPost { get; set; }
    public bool DeleteOwnerPost { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool Moderator { get; set; }
    public bool NoviceUser { get; set; }
    public DateTime ChangeDate { get; set; }
  }
}
