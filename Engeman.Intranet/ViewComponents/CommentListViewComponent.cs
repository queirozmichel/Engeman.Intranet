using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Engeman.Intranet.ViewComponents
{
  public class CommentListViewComponent : ViewComponent
  {
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostCommentFileRepository _postCommentFileRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public CommentListViewComponent(IPostCommentRepository postCommentRepository, IUserAccountRepository userAccountRepository, IPostCommentFileRepository postCommentFileRepository, IDepartmentRepository departmentRepository)
    {
      _postCommentRepository = postCommentRepository;
      _userAccountRepository = userAccountRepository;
      _postCommentFileRepository = postCommentFileRepository;
      _departmentRepository = departmentRepository;
    }

    public IViewComponentResult Invoke(int postId)
    {
      var comments = new List<PostCommentViewModel>();
      var userAccount = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      var orderedPostComments = _postCommentRepository.GetPostCommentsByRestriction(userAccount, postId);

      for (int i = 0; i < orderedPostComments.Count; i++)
      {
        var postCommentViewModel = new PostCommentViewModel();
        var userAccountComment = _userAccountRepository.GetUserAccountById(orderedPostComments[i].UserAccountId);
        postCommentViewModel.Id = orderedPostComments[i].Id;
        postCommentViewModel.Description = orderedPostComments[i].Description;
        postCommentViewModel.Username = userAccountComment.Name;
        postCommentViewModel.Photo = userAccountComment.Photo;
        postCommentViewModel.UserId = orderedPostComments[i].UserAccountId;
        postCommentViewModel.DepartmentName = _departmentRepository.GetDepartmentNameById(orderedPostComments[i].DepartmentId);
        postCommentViewModel.ChangeDate = orderedPostComments[i].ChangeDate;
        postCommentViewModel.Revised = orderedPostComments[i].Revised;
        postCommentViewModel.Files = _postCommentFileRepository.GetFilesByPostCommentId(orderedPostComments[i].Id).OrderBy(x => x.Name).ToList();
        comments.Add(postCommentViewModel);
      }

      ViewBag.PostComments = comments;
      ViewBag.UserAccount = userAccount;
      ViewBag.PostId = postId;

      return View();
    }
  }
}