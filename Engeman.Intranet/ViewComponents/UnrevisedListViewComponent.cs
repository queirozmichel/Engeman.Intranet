﻿using Engeman.Intranet.Extensions;
using Engeman.Intranet.Helpers;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
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
      var username = HttpContext.Session.Get<string>("_CurrentUsername");
      var user = _userAccount.GetByUsername (username);
      ViewBag.UnrevisedPosts = _postRepository.GetPostsGrid(user).AsQueryable().Where("revised == (@0)", false).Count();
      ViewBag.UnrevisedComments = _postCommentRepository.GetUnrevisedComments().Count();
      ViewBag.Moderator = GlobalFunctions.IsModerator(user.Id);

      return View(user);
    }
  }
}
