﻿using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
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
    private readonly ICommentRepository _postCommentRepository;
    private readonly ICommentFileRepository _postCommentFileRepository;
    private readonly IUserAccountRepository _userAccountRepository;

    public CommentsController(ICommentRepository postCommentRepository, IPostFileRepository postFileRepository, ICommentFileRepository postCommentFileRepository, IPostRepository postRepository, IUserAccountRepository userAccountRepository, IDepartmentRepository departmentRepository)
    {
      _postCommentRepository = postCommentRepository;
      _postCommentFileRepository = postCommentFileRepository;
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
      var currentComment = _postCommentRepository.GetById(editedComment.Comment.Id);
      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetByDomainUsername(sessionDomainUsername);

      for (int i = 0; i < editedComment.Files.Count; i++)
      {
        if (editedComment.Files[i].Active == false)
        {
          _postCommentFileRepository.Delete(editedComment.Files[i].Id);
          editedComment.Files.RemoveAt(i);
          i--;
        }
      }

      comment.Description = editedComment.Comment.Description;

      if (currentComment.Revised == true && userAccount.NoviceUser == false)
      {
        comment.Revised = true;
      }

      _postCommentRepository.Update(currentComment.Id, comment);

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
        _postCommentFileRepository.Add(currentComment.Id, files);
      }
      return RedirectToAction("PostDetails", "Posts", new { postId = editedComment.Comment.PostId });
    }

    [HttpGet]
    public IActionResult ShowFile(int commentId, int file)
    {
      var orderedFiles = _postCommentFileRepository.GetByCommentId(commentId).OrderBy(a => a.Name).ToList();
      //Adiciona "inline" no cabeçalho da página ao invés de "attachment" para forçar abrir ao invés de baixar
      Response.Headers.Add("Content-Disposition", "inline; filename=" + orderedFiles[file].Name);

      return File(orderedFiles[file].BinaryData, "application/pdf");
    }

    [HttpPost]
    public IActionResult NewComment(NewCommentViewModel newComment, List<IFormFile> files)
    {
      if (!ModelState.IsValid)
      {
        return Ok(-1);
      }

      var sessionDomainUsername = HttpContext.Session.GetString("_DomainUsername");
      var userAccount = _userAccountRepository.GetByDomainUsername(sessionDomainUsername);

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
      _postCommentRepository.Add(newComment);

      return Ok(1);
    }

    [HttpDelete]
    public IActionResult DeleteComment(int commentId)
    {
      var result = _postCommentRepository.Delete(commentId);
      return Json(result);
    }

    [HttpPut]
    public IActionResult AproveComment(int commentId)
    {
      var comment = _postCommentRepository.GetById(commentId);
      comment.Revised = true;
      _postCommentRepository.Update(commentId, comment);

      return ViewComponent("UnrevisedList");
    }
  }
}