﻿namespace Engeman.Intranet.Models
{
  public class UserPermissionsViewModel
  {
    public bool CreatePost { get; set; }
    public bool EditOwnerPost { get; set; }
    public bool DeleteOwnerPost { get; set; }
    public bool EditAnyPost { get; set; }
    public bool DeleteAnyPost { get; set; }
  }
}