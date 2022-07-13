using System;
using System.ComponentModel.DataAnnotations;

namespace Engeman.Intranet.Models
{
  public class Post
  {
    public Post()
    {
      this.UserAccount = new UserAccount();
      this.Department = new Department();
    }
    public int Id { get; set; }
    public char Active { get; set; }
    [Required]
    public char Restricted { get; set; }
    [Required]
    public string Subject { get; set; }
    [Required]
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public int UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public DateTime ChangeDate { get; set; }
    public char PostType { get; set; }
    public bool CheckIsRestricted
    {
      get
      {
        if (Restricted == 'N')
        {
          return false;
        }
        else
        {
          return true;
        }
      }
      set
      {
        if (value == false)
        {
          Restricted = 'N';
        }
        else
        {
          Restricted = 'S';
        }
      }
    }
  }
}
