using System;

namespace Engeman.Intranet.Models
{
  public class UserProfileDto
  {
    public int Id{ get; set; }
    public string Name { get; set; }
    public string DomainAccount { get; set; }
    public string Email { get; set; }
    public string ChangeDate { get; set; }
  }
}
