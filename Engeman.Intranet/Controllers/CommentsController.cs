using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engeman.Intranet.Controllers
{
  [Authorize(AuthenticationSchemes = "CookieAuthentication")]
  public class CommentsController : Controller
  {
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IPostCommentFileRepository _postCommentFileRepository;
    private readonly IPostRepository _postRepository;
    private readonly IUserAccountRepository _userAccountRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public CommentsController(IPostCommentRepository postCommentRepository, IPostFileRepository postFileRepository, IPostCommentFileRepository postCommentFileRepository, IPostRepository postRepository, IUserAccountRepository userAccountRepository, IDepartmentRepository departmentRepository)
    {
      _postCommentRepository = postCommentRepository;
      _postCommentFileRepository = postCommentFileRepository;
      _postRepository = postRepository;
      _userAccountRepository = userAccountRepository;
      _departmentRepository = departmentRepository;
    }
   
    public IActionResult Index()
    {
      return View();
    }

    [HttpGet]
    public IActionResult NewComment()
    {
      return ViewComponent("WangEditor");
    }

    public IActionResult CommentList(int idPost)
    {
      return ViewComponent("CommentList", idPost);
    }

    public IActionResult CommentEditForm(int commentId)
    {
      return ViewComponent("CommentEditForm", commentId);
    }

    [HttpPost]
    public IActionResult UpdateComment(CommentEditViewModel editedComment, List<IFormFile> binaryData)
    {
      List<PostCommentFile> files = new List<PostCommentFile>();
      PostComment comment = new PostComment();
      var currentComment = _postCommentRepository.GetPostCommentById(editedComment.Comment.Id);
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetUserAccountByDomainUsername(sessionDomainUsername);

      for (int i = 0; i < editedComment.Files.Count; i++)
      {
        if (editedComment.Files[i].Active == 'N')
        {
          _postCommentFileRepository.DeleteFileById(editedComment.Files[i].Id);
          editedComment.Files.RemoveAt(i);
          i--;
        }
      }

      for (int i = 0; i < editedComment.Files.Count; i++)
      {
        PostCommentFile fileUpdate = new PostCommentFile();
        fileUpdate.Description = editedComment.Comment.Description;
        files.Add(fileUpdate);
      }

      comment.Description = editedComment.Comment.Description;
      comment.CleanDescription = editedComment.Comment.Description;
      comment.Keywords = editedComment.Comment.Keywords;

      if (currentComment.Revised == true && userAccount.NoviceUser == false)
      {
        comment.Revised = true;
      }

      _postCommentRepository.UpdatePostCommentById(currentComment.Id, comment, files);

      if (binaryData.Count != 0)
      {
        files.Clear();

        for (int i = 0; i < binaryData.Count; i++)
        {
          PostCommentFile newFile = new PostCommentFile();
          if (binaryData[i].Length > 0)
          {
            using (var stream = new MemoryStream())
            {
              binaryData[i].CopyTo(stream);
              newFile.BinaryData = stream.ToArray();
              newFile.Name = binaryData[i].FileName;
              newFile.Description = comment.Description;
              files.Add(newFile);
            }
          }
        }
        _postCommentFileRepository.AddFilesToComment(currentComment.Id, files);
      }

      return RedirectToAction("CommentList", new { idPost = editedComment.Comment.PostId });
    }

    [HttpGet]
    public IActionResult ShowCommentFile(int idComment, int file)
    {
      var orderedFiles = _postCommentFileRepository.GetFilesByPostCommentId(idComment).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + orderedFiles[file].Name);

      return File(orderedFiles[file].BinaryData, "application/pdf");
    }
  }
}