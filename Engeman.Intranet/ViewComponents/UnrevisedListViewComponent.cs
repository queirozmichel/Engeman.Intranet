﻿using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Engeman.Intranet.ViewComponents
{
  public class UnrevisedListViewComponent : ViewComponent
  {
    private readonly IUserAccountRepository _userAccount;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _postCommentRepository;

    public UnrevisedListViewComponent(IUserAccountRepository userAccount, IPostRepository postRepository, ICommentRepository postCommentRepository)
    {
      _userAccount = userAccount;
      _postRepository = postRepository;
      _postCommentRepository = postCommentRepository;
    }

    public IViewComponentResult Invoke()
    {
      var userDomain = HttpContext.Session.GetString("_DomainUsername");
      var user = _userAccount.GetByDomainUsername (userDomain);
      ViewBag.UnrevisedPosts = _postRepository.GetByRestriction(user).AsQueryable().Where("revised == (@0)", false).Count();
      ViewBag.UnrevisedComments = _postCommentRepository.GetUnrevisedComments().Count();

      return View(user);
    }
  }
}
