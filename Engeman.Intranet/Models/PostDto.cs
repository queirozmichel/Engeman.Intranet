namespace Engeman.Intranet.Models
{
  public class PostDto
  {
    public int Id { get; set; }
    public bool Active { get; set; }
    public bool Restricted { get; set; }
    public bool Revised { get; set; }
    public string Subject { get; set; }
    public string Description { get; set; }
    public string CleanDescription { get; set; }
    public string Keywords { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentDescription { get; set; }
    public int UserAccountId { get; set; }
    public string UserAccountName { get; set; }
    public string ChangeDate { get; set; }
    public char PostType { get; set; }
    public bool HasUnrevisedComments { get; set; }
  }
}