using Engeman.Intranet.Extensions;
using Engeman.Intranet.Helpers;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class CommentsController : RootController
  {
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentFileRepository _commentFileRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IPostRestrictionRepository _postRestrictionRepository;
    private readonly INotificationRepository _notificationRepository;

    public CommentsController(ICommentRepository commentRepository, ICommentFileRepository commentFileRepository, IUserAccountRepository userAccountRepository, IPostRepository postRepository, IPostRestrictionRepository postRestrictionRepository, INotificationRepository notificationRepository)
    {
      _commentRepository = commentRepository;
      _commentFileRepository = commentFileRepository;
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _postRestrictionRepository = postRestrictionRepository;
      _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public IActionResult WangEditor()
    {
      return ViewComponent("WangEditor");
    }

    [HttpGet]
    public IActionResult CommentEditForm(int commentId)
    {
      if (!HttpContext.Request.IsAjaxOrFetch("GET")) return Redirect(Request.Host.ToString());

      return ViewComponent("CommentEditForm", commentId);
    }

    [HttpPost]
    public IActionResult UpdateComment(CommentEditViewModel editedComment, List<IFormFile> binaryData)
    {
      var files = new List<CommentFile>();
      var comment = new Comment();
      var currentPost = new Post();
      var currentComment = new Comment();
      var currentUserId = HttpContext.Session.Get<int>("_CurrentUserId");
      var userAccount = new UserAccount();
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      int[] filesToBeRemove = null;

      if (Request.Cookies["FilesToBeRemove"] != string.Empty)
      {
        filesToBeRemove = (Regex.Replace(Request.Cookies["FilesToBeRemove"], @"(^\s*)|(\s*$)", "")).Split(' ').Select(int.Parse).ToArray();
      }

      try
      {
        currentComment = _commentRepository.GetById(editedComment.Comment.Id);
        userAccount = _userAccountRepository.GetByUsername(sessionUsername);
        currentPost = _postRepository.GetById(editedComment.Comment.PostId);
      }
      catch (Exception) { }

      comment.Description = editedComment.Comment.Description;

      comment.CleanDescription = GlobalFunctions.CleanText(GlobalFunctions.HTMLToTextConvert(editedComment.Comment.Description));

      if (!GlobalFunctions.RequiresModeration(currentUserId, currentPost.PostType)) comment.Revised = true;

      if (binaryData.Count != 0)
      {
        files.Clear();
        for (int i = 0; i < binaryData.Count; i++)
        {
          var newFile = new CommentFile();
          if (binaryData[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              binaryData[i].CopyTo(stream);
              newFile.BinaryData = stream.ToArray();
              newFile.Name = binaryData[i].FileName;
              files.Add(newFile);
            }
          }
        }
      }

      try
      {
        _commentRepository.Update(currentComment.Id, comment, sessionUsername);
        if (filesToBeRemove != null) _commentFileRepository.Delete(filesToBeRemove);
        _commentFileRepository.Add(currentComment.Id, files);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }

      return RedirectToAction("PostDetails", "Posts", new { postId = editedComment.Comment.PostId });
    }

    [HttpGet]
    public IActionResult ShowFile(int commentId, int file)
    {
      if (HttpContext.Session.Get<bool>("_IsModerator") == false)
      {
        var postId = _commentRepository.GetPostIdById(commentId);
        var post = _postRepository.GetById(postId);
        if (post.Restricted == true)
        {
          var departmentId = _userAccountRepository.GetDepartmentIdById(HttpContext.Session.Get<int>("_CurrentUserId"));
          var postRestrictionCount = _postRestrictionRepository.CountByPostIdDepId(postId, departmentId);
          if (postRestrictionCount == 0)
          {
            return Redirect(Request.Host.ToString());
          }
        }
      }

      var orderedFiles = new List<CommentFile>();

      try { orderedFiles = _commentFileRepository.GetByCommentId(commentId).OrderBy(a => a.Name).ToList(); } catch (Exception) { }

      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + Uri.EscapeDataString(orderedFiles[file].Name));

      return File(orderedFiles[file].BinaryData, "application/pdf");
    }

    [HttpPost]
    public IActionResult NewComment(NewCommentViewModel newComment, List<IFormFile> files)
    {
      var sessionUsername = HttpContext.Session.Get<string>("_CurrentUsername");
      var userAccount = new UserAccount();
      var postAux = new Post();

      try
      {
        userAccount = _userAccountRepository.GetByUsername(sessionUsername);
        postAux = _postRepository.GetById(newComment.PostId);
      }
      catch (Exception) { }

      newComment.Revised = !GlobalFunctions.RequiresModeration(HttpContext.Session.Get<int>("_CurrentUserId"), postAux.PostType);

      newComment.CleanDescription = GlobalFunctions.CleanText(GlobalFunctions.HTMLToTextConvert(newComment.Description));

      newComment.UserAccountId = HttpContext.Session.Get<int>("_CurrentUserId");

      if (files.Count > 0)
      {
        for (int i = 0; i < files.Count; i++)
        {
          var file = new NewCommentFileViewModel();
          file.Name = files[i].FileName;
          if (files[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              files[i].CopyTo(stream);
              file.BinaryData = stream.ToArray();
            }
          }
          newComment.Files.Add(file);
        }
      }

      try
      {
        var commentOutputId = _commentRepository.Add(newComment, sessionUsername);

        if (postAux.UserAccountId != newComment.UserAccountId)
        {
          _notificationRepository.Add(CreateNotification(commentOutputId, postAux.UserAccountId, 1, newComment.Revised));
        }
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpDelete]
    public IActionResult DeleteComment(int commentId)
    {
      if (!HttpContext.Request.IsAjaxOrFetch("DELETE")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _commentRepository.Delete(commentId, currentUsername); } catch (Exception) { }

      return ViewComponent("UnrevisedList", StatusCodes.Status200OK);
    }

    [HttpPut]
    public IActionResult AproveComment(int commentId)
    {
      if (!HttpContext.Request.IsAjaxOrFetch("PUT")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try
      {
        _commentRepository.Aprove(commentId, currentUsername);
        _notificationRepository.MakeRevisedByRegistryId(commentId);
      }
      catch (Exception) { }

      return ViewComponent("UnrevisedList");
    }

    public int GetPostIdByCommentId(int commentId)
    {
      try
      {
        return _commentRepository.GetPostIdById(commentId);
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}