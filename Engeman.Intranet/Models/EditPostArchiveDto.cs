using System;

namespace Engeman.Intranet.Models
{
  public class EditPostArchiveDto
  {

    public Post Post { get; set; }
    public Archive Archive { get; set; }

    //  public int IdPost { get; set; }
    //  public char Restricted { get; set; }
    //  public string Subject { get; set; }
    //  public string Description { get; set; }
    //  public string CleanDescription { get; set; }
    //  public string Keywords { get; set; }
    //  public int UserAccountId { get; set; }
    //  public int DepartmentId { get; set; }
    //  public int IdArchive { get; set; }
    //  public char ArchiveType { get; set; }
    //  public string Name { get; set; }
    //  public byte[] BinaryData { get; set; }
    //  public int PostId { get; set; }
    //  public bool CheckIsRestricted
    //  {
    //    get
    //    {
    //      if (Restricted == 'N')
    //      {
    //        return false;
    //      }
    //      else
    //      {
    //        return true;
    //      }
    //    }
    //    set
    //    {
    //      if (value == false)
    //      {
    //        Restricted = 'N';
    //      }
    //      else
    //      {
    //        Restricted = 'S';
    //      }
    //    }
    //  }
    //}
  }
}

