﻿using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;

namespace Engeman.Intranet.Repositories
{
  public interface IBlacklistTermRepository
  {
    public List<BlacklistTermViewModel> GetBlacklistTermsGrid();
    public List<BlacklistTerm> Get();
    public BlacklistTerm GetById(int id);
    public void Add(BlacklistTermViewModel term, string currentUsername = null);
    public void Update(int id, BlacklistTerm term, string currentUsername = null);
    public void Delete(int id, string currentUsername = null);
  }
}
