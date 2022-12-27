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

    public CommentsController(ICommentRepository commentRepository, IPostFileRepository postFileRepository, ICommentFileRepository commentFileRepository, IPostRepository postRepository, IUserAccountRepository userAccountRepository, IDepartmentRepository departmentRepository)
    {
      _commentRepository = commentRepository;
      _commentFileRepository = commentFileRepository;
      _userAccountRepository = userAccountRepository;
    }
   
    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public IActionResult WangEditor()
    {
      return ViewComponent("WangEditor");
    }

    public IActionResult CommentEditForm(int commentId)
    {
      return ViewComponent("CommentEditForm", commentId);
    }

    [HttpPost]
    public IActionResult UpdateComment(CommentEditViewModel editedComment, List<IFormFile> binaryData)
    {
      List<CommentFile> files = new List<CommentFile>();
      Comment comment = new Comment();
      var currentComment = _commentRepository.GetById(editedComment.Comment.Id);
      var sessionUsername = HttpContext.Session.GetString("_Username");
      var userAccount = _userAccountRepository.GetByUsername(sessionUsername);

      for (int i = 0; i < editedComment.Files.Count; i++)
      {
        if (editedComment.Files[i].Active == false)
        {
          _commentFileRepository.Delete(editedComment.Files[i].Id);
          editedComment.Files.RemoveAt(i);
          i--;
        }
      }

      comment.Description = editedComment.Comment.Description;

      if (currentComment.Revised == true && userAccount.NoviceUser == false)
      {
        comment.Revised = true;
      }

      _commentRepository.UpdateWithLog(currentComment.Id, comment, sessionUsername);

      if (binaryData.Count != 0)
      {
        files.Clear();

        for (int i = 0; i < binaryData.Count; i++)
        {
          CommentFile newFile = new CommentFile();
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
        _commentFileRepository.Add(currentComment.Id, files);
      }
      return RedirectToAction("PostDetails", "Posts", new { postId = editedComment.Comment.PostId });
    }

    [HttpGet]
    public IActionResult ShowFile(int commentId, int file)
    {
      var orderedFiles = _commentFileRepository.GetByCommentId(commentId).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + Uri.EscapeDataString(orderedFiles[file].Name));
      return File(orderedFiles[file].BinaryData, "application/pdf");
    }

    [HttpPost]
    public IActionResult NewComment(NewCommentViewModel newComment, List<IFormFile> files)
    {
      if (!ModelState.IsValid)
      {
        return Ok(-1);
      }

      var sessionUsername = HttpContext.Session.GetString("_Username");
      var userAccount = _userAccountRepository.GetByUsername(sessionUsername);

      if (userAccount.Moderator == true || userAccount.NoviceUser == false)
      {
        newComment.Revised = true;
      }

      newComment.UserAccountId = (int)HttpContext.Session.GetInt32("_UserAccountId");

      if (files.Count > 0)
      {
        for (int i = 0; i < files.Count; i++)
        {
          NewCommentFileViewModel file = new NewCommentFileViewModel();
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
      _commentRepository.AddWithLog(newComment, sessionUsername);

      return Ok(1);
    }

    [HttpDelete]
    public IActionResult DeleteComment(int commentId)
    {
      var currentUsername = HttpContext.Session.GetString("_Username");
      var result = _commentRepository.DeleteWithLog(commentId, currentUsername);
      return Json(result);
    }

    [HttpPut]
    public IActionResult AproveComment(int commentId)
    {
      var currentUsername = HttpContext.Session.GetString("_Username");
      _commentRepository.AproveWithLog(commentId, currentUsername);      

      return ViewComponent("UnrevisedList");
    }
  }
}