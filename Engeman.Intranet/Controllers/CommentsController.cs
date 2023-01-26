using Engeman.Intranet.Extensions;
﻿using Engeman.Intranet.Models;
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
      var files = new List<CommentFile>();
      var comment = new Comment();
      var currentComment = new Comment();
      var userAccount = new UserAccount();
      var sessionUsername = HttpContext.Session.Get<string>("_Username");

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
      var sessionUsername = HttpContext.Session.Get<string>("_Username");
      var userAccount = new UserAccount();

      try { userAccount = _userAccountRepository.GetByUsername(sessionUsername); }
      catch (Exception) { }
      if (userAccount.Moderator == true || userAccount.NoviceUser == false) newComment.Revised = true;
      newComment.UserAccountId = HttpContext.Session.Get<int>("_CurrentUserId");
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
      var currentUsername = HttpContext.Session.Get<string>("_Username");

      try { _commentRepository.DeleteWithLog(commentId, currentUsername); }
      catch (Exception) { }
      return Ok(StatusCodes.Status200OK);
    }

    [HttpPut]
    public IActionResult AproveComment(int commentId)
    {
      var currentUsername = HttpContext.Session.Get<string>("_Username");

      return ViewComponent("UnrevisedList");
    }
  }
}