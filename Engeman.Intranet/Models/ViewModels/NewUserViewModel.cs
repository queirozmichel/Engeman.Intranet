namespace Engeman.Intranet.Models.ViewModels
{
  public class NewUserViewModel
  {
    public NewUserViewModel(string name, string username, int departmentId, int permission)
    {
      Name = name;
      Username = username;
      DepartmentId = departmentId;
      Email = username + "@engeman.com.br";
      SetPermissions(permission);
    }

    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public int Moderator { get; set; }
    public int NoviceUser { get; set; }
    public int DepartmentId { get; set; }

    public void SetPermissions(int permission)
    {
      if (permission == 0)
      {
        Moderator = 0;
        NoviceUser = 0;
      }
      else if (permission == 1)
      {
        Moderator = 0;
        NoviceUser = 1;
      }
      else if (permission == 3)
      {
        Moderator = 1;
        NoviceUser = 0;
      }
    }
  }
}