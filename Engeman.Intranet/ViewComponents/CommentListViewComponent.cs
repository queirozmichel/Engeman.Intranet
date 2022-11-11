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
    private readonly IPostRepository _postRepository;
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostCommentFileRepository _postCommentFileRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public CommentListViewComponent(IPostCommentRepository postCommentRepository, IUserAccountRepository userAccountRepository, IPostCommentFileRepository postCommentFileRepository, IDepartmentRepository departmentRepository, IPostRepository postRepository)
    {
      _postCommentRepository = postCommentRepository;
      _userAccountRepository = userAccountRepository;
      _postCommentFileRepository = postCommentFileRepository;
      _departmentRepository = departmentRepository;
      _postRepository = postRepository;
    }

    public IViewComponentResult Invoke(int postId)
    {
      var comments = new List<CommentViewModel>();
      var userAccount = _userAccountRepository.GetUserAccountById((int)HttpContext.Session.GetInt32("_UserAccountId"));
      var orderedComments = _postCommentRepository.GetPostCommentsByRestriction(userAccount, postId);

      List<CommentFile> commentFiles = new List<CommentFile>();

      for (int i = 0; i < orderedComments.Count; i++)
      {
        var comment = new CommentViewModel();
        var authorComment = _userAccountRepository.GetUserAccountById(orderedComments[i].UserAccountId);
        commentFiles = _postCommentFileRepository.GetFilesByPostCommentId(orderedComments[i].Id).OrderBy(x => x.Name).ToList();
        comment.Id = orderedComments[i].Id;
        comment.Description = orderedComments[i].Description;
        comment.AuthorUsername = authorComment.Name;
        comment.AuthorPhoto = authorComment.Photo;
        comment.AuthorId = orderedComments[i].UserAccountId;
        comment.AuthorDepartment = _departmentRepository.GetDepartmentNameById(orderedComments[i].DepartmentId);
        comment.AuthorPostsMade = _postRepository.GetPostsByUserId(authorComment.Id).Count();
        comment.AuthorCommentsMade = _postCommentRepository.GetPostCommentsByUserId(authorComment.Id).Count();
        comment.ChangeDate = orderedComments[i].ChangeDate;
        comment.Revised = orderedComments[i].Revised;
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