using System;

namespace Engeman.Intranet.Models
{
  public class AskQuestionDto
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public char Restricted { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public int UserAccountId { get; set; }
    public string DomainAccount { get; set; }
    public int DepartmentId { get; set; }
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
