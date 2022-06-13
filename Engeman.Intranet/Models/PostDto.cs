namespace Engeman.Intranet.Models
{
  public class PostDto
  {
    public int Id { get; set; }
    public char Active { get; set; }
    public string Subject { get; set; }
    public string CleanDescription { get; set; }
    public string DepartmentDescription { get; set; }
    public string UserAccountName { get; set; }
    public string ChangeDate { get; set; }
  }
}