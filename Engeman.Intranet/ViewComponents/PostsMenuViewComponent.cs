﻿using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Engeman.Intranet.ViewComponents
{
  public class PostsMenuViewComponent : ViewComponent
  {
    private readonly IUserAccountRepository _userAccount;
    private readonly IPostRepository _postRepository;
    private readonly IPostCommentRepository _postCommentRepository;

    public PostsMenuViewComponent(IUserAccountRepository userAccount, IPostRepository postRepository, IPostCommentRepository postCommentRepository)
    {
      _userAccount = userAccount;
      _postRepository = postRepository;
      _postCommentRepository = postCommentRepository;
    }

    public IViewComponentResult Invoke()
    {
      var userDomain = HttpContext.Session.GetString("_DomainUsername");
      var user = _userAccount.GetUserAccountByDomainUsername (userDomain);
      ViewBag.UnrevisedPosts = _postRepository.GetPostsByRestriction(user).AsQueryable().Where("revised == (@0)", false).Count();
      ViewBag.UnrevisedComments = _postCommentRepository.GetUnrevisedComments().Count();

      return View(user);
    }
  }
}
