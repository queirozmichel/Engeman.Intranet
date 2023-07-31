namespace Engeman.Intranet.Models.ViewModels
{
  public class UserPermissionsViewModel
  {
    public PostType PostType { get; set; }
  }

  public class PostType
  {
    public Informative Informative { get; set; }
    public Question Question { get; set; }
    public Manual Manual { get; set; }
    public Document Document { get; set; }
  }

  public class Informative
  {
    public int CanPost { get; set; }
    public int CanComment { get; set; }
    public int EditAnyPost { get; set; }
    public int DeleteAnyPost { get; set; }
    public int RequiresModeration { get; set; }
  }

  public class Question
  {
    public int CanPost { get; set; }
    public int CanComment { get; set; }
    public int EditAnyPost { get; set; }
    public int DeleteAnyPost { get; set; }
    public int RequiresModeration { get; set; }
  }

  public class Manual
  {
    public int CanPost { get; set; }
    public int CanComment { get; set; }
    public int EditAnyPost { get; set; }
    public int DeleteAnyPost { get; set; }
    public int RequiresModeration { get; set; }
  }

  public class Document
  {
    public int CanPost { get; set; }
    public int CanComment { get; set; }
    public int EditAnyPost { get; set; }
    public int DeleteAnyPost { get; set; }
    public int RequiresModeration { get; set; }
  }
}
