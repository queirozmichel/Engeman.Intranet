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
  public class CommentsController : Controller
  {
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentFileRepository _commentFileRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IPostRepository _postRepository;
    private readonly IPostRestrictionRepository _postRestrictionRepository;

    public CommentsController(ICommentRepository commentRepository, ICommentFileRepository commentFileRepository, IUserAccountRepository userAccountRepository, IPostRepository postRepository, IPostRestrictionRepository postRestrictionRepository)
    {
      _commentRepository = commentRepository;
      _commentFileRepository = commentFileRepository;
      _userAccountRepository = userAccountRepository;
      _postRepository = postRepository;
      _postRestrictionRepository = postRestrictionRepository;
    }

    [HttpGet]
    public IActionResult WangEditor()
    {
      return ViewComponent("WangEditor");
    }

    [HttpGet]
    public IActionResult CommentEditForm(int commentId)
    {
      if (!HttpContext.Request.IsAjax("GET")) return Redirect(Request.Host.ToString());

      return ViewComponent("CommentEditForm", commentId);
    }

    [HttpPost]
    public IActionResult UpdateComment(CommentEditViewModel editedComment, List<IFormFile> binaryData)
    {
      var files = new List<CommentFile>();
      var comment = new Comment();
      var currentComment = new Comment();
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
      }
      catch (Exception) { }

      comment.Description = editedComment.Comment.Description;

      comment.CleanDescription = GlobalFunctions.CleanText(GlobalFunctions.HTMLToTextConvert(editedComment.Comment.Description));

      if (currentComment.Revised == true && userAccount.NoviceUser == false) comment.Revised = true;

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

      try { userAccount = _userAccountRepository.GetByUsername(sessionUsername); } catch (Exception) { }

      if (userAccount.Moderator == true || userAccount.NoviceUser == false) newComment.Revised = true;

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

      try { _commentRepository.Add(newComment, sessionUsername); }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
      }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpDelete]
    public IActionResult DeleteComment(int commentId)
    {
      if (!HttpContext.Request.IsAjax("DELETE")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _commentRepository.Delete(commentId, currentUsername); } catch (Exception) { }

      return ViewComponent("UnrevisedList", StatusCodes.Status200OK);
    }

    [HttpPut]
    public IActionResult AproveComment(int commentId)
    {
      if (!HttpContext.Request.IsAjax("PUT")) return Redirect(Request.Host.ToString());

      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _commentRepository.Aprove(commentId, currentUsername); } catch (Exception) { }

      return ViewComponent("UnrevisedList");
    }
  }
}