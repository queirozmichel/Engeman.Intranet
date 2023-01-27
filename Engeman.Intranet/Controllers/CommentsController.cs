using Engeman.Intranet.Extensions;
using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class CommentsController : Controller
  {
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentFileRepository _commentFileRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public CommentsController(ICommentRepository commentRepository, ICommentFileRepository commentFileRepository, IUserAccountRepository userAccountRepository)
    {
      _commentRepository = commentRepository;
      _commentFileRepository = commentFileRepository;
      _userAccountRepository = userAccountRepository;
    }

    [HttpGet]
    public IActionResult WangEditor()
    {
      return ViewComponent("WangEditor");
    }

    [HttpGet]
    public IActionResult CommentEditForm(int commentId)
    {
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

      try
      {
        currentComment = _commentRepository.GetById(editedComment.Comment.Id);
        userAccount = _userAccountRepository.GetByUsername(sessionUsername);
      }
      catch (Exception) { }

      for (int i = 0; i < editedComment.Files.Count; i++)
      {
        if (editedComment.Files[i].Active == false)
        {
          try { _commentFileRepository.Delete(editedComment.Files[i].Id); } catch (Exception) { }

          editedComment.Files.RemoveAt(i);
          i--;
        }
      }

      comment.Description = editedComment.Comment.Description;

      if (currentComment.Revised == true && userAccount.NoviceUser == false) comment.Revised = true;

      try { _commentRepository.UpdateWithLog(currentComment.Id, comment, sessionUsername); } catch (Exception) { }

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

        try { _commentFileRepository.Add(currentComment.Id, files); } catch (Exception) { }
      }

      return RedirectToAction("PostDetails", "Posts", new { postId = editedComment.Comment.PostId });
    }

    [HttpGet]
    public IActionResult ShowFile(int commentId, int file)
    {
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

      try { _commentRepository.AddWithLog(newComment, sessionUsername); } catch (Exception) { }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpDelete]
    public IActionResult DeleteComment(int commentId)
    {
      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _commentRepository.DeleteWithLog(commentId, currentUsername); } catch (Exception) { }

      return Ok(StatusCodes.Status200OK);
    }

    [HttpPut]
    public IActionResult AproveComment(int commentId)
    {
      var currentUsername = HttpContext.Session.Get<string>("_CurrentUsername");

      try { _commentRepository.AproveWithLog(commentId, currentUsername); } catch (Exception) { }

      return ViewComponent("UnrevisedList");
    }
  }
}