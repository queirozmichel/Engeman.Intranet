﻿using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRestrictionRepository
  {
    public List<int> GetDepartmentsByIdPost(int idPost);
  }
}