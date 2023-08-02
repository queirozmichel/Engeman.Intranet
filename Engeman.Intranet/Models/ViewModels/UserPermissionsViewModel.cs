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
    public bool CanPost { get; set; }
    public bool CanComment { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool RequiresModeration { get; set; }
  }

  public class Question
  {
    public bool CanPost { get; set; }
    public bool CanComment { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool RequiresModeration { get; set; }
  }

  public class Manual
  {
    public bool CanPost { get; set; }
    public bool CanComment { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool RequiresModeration { get; set; }
  }

  public class Document
  {
    public bool CanPost { get; set; }
    public bool CanComment { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
    public bool RequiresModeration { get; set; }
  }
}
