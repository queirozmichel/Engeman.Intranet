namespace Engeman.Intranet.Models
{
  public class PostDto
  {
    public PostDto()
    {
      this.UserAccount = new UserAccount();
      this.Department = new Department();
    }
    public int Id { get; set; }
    public char Active { get; set; }
    public string Subject { get; set; }
    public string CleanDescription { get; set; }
    public int UserAccountId { get; set; }
    public UserAccount UserAccount { get; set; }
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public string ChangeDate { get; set; }
  }
}