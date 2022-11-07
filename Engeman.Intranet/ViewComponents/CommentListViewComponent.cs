using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
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
      var comments = new List<CommentViewModel>();
      var userAccount = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      var orderedPostComments = _postCommentRepository.GetPostCommentsByRestriction(userAccount, postId);
      List<CommentFile> commentFiles = new List<CommentFile>();

      for (int i = 0; i < orderedPostComments.Count; i++)
      {
        var comment = new CommentViewModel();
        var userAccountComment = _userAccountRepository.GetUserAccountById(orderedPostComments[i].UserAccountId);
        commentFiles = _postCommentFileRepository.GetFilesByPostCommentId(orderedPostComments[i].Id).OrderBy(x => x.Name).ToList();
        comment.Id = orderedPostComments[i].Id;
        comment.Description = orderedPostComments[i].Description;
        comment.Username = userAccountComment.Name;
        comment.UserPhoto = userAccountComment.Photo;
        comment.UserId = orderedPostComments[i].UserAccountId;
        comment.UserDepartmentName = _departmentRepository.GetDepartmentNameById(orderedPostComments[i].DepartmentId);
        comment.ChangeDate = orderedPostComments[i].ChangeDate;
        comment.Revised = orderedPostComments[i].Revised;
        comment.Files = commentFiles;
        comments.Add(comment);
      }

      ViewBag.PostComments = comments;
      ViewBag.UserAccount = userAccount;
      ViewBag.PostId = postId;

      return View();
    }
  }
}