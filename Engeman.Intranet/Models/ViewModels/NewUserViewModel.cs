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
    public string Permissions { get; set; }
    public int DepartmentId { get; set; }

    public void SetPermissions(int permission)
    {
      if (permission == 0)
      {
        Permissions = "{\"PostType\":{\"Informative\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":0},\"Question\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":0},\"Manual\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":0},\"Document\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":0}}}";
      }
      else if (permission == 1)
      {
        Permissions = "{\"PostType\":{\"Informative\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":1},\"Question\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":1},\"Manual\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":1},\"Document\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":0,\"DeleteAnyPost\":0,\"RequiresModeration\":1}}}";
      }
      else if (permission == 2)
      {
        Permissions = "{\"PostType\":{\"Informative\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":1,\"DeleteAnyPost\":1,\"RequiresModeration\":0},\"Question\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":1,\"DeleteAnyPost\":1,\"RequiresModeration\":0},\"Manual\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":1,\"DeleteAnyPost\":1,\"RequiresModeration\":0},\"Document\":{\"CanPost\":1,\"CanComment\":1,\"EditAnyPost\":1,\"DeleteAnyPost\":1,\"RequiresModeration\":0}}}";
      }
    }
  }
}