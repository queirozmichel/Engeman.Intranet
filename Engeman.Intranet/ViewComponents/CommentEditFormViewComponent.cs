using Engeman.Intranet.Models;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.Design;
using System.Linq;

namespace Engeman.Intranet.ViewComponents
{
  public class CommentEditFormViewComponent : ViewComponent
  {
    private readonly IPostCommentRepository _postCommentRepository;
    private readonly IPostCommentFileRepository _postCommentFileRepository;

    public CommentEditFormViewComponent(IPostCommentRepository postCommentRepository, IPostCommentFileRepository postCommentFileRepository)
    {
      _postCommentRepository = postCommentRepository;
      _postCommentFileRepository = postCommentFileRepository;
    }

    public IViewComponentResult Invoke(int commentId)
    {
      String selectorId = "";
      Random res = new Random();
      String str = "abcdefghijklmnopqrstuvwxyz";
      int size = 5;

      for (int i = 0; i < size; i++)
      {
        int x = res.Next(str.Length);
        selectorId = selectorId + str[x];
      }
 
      ViewBag.SelectorId = selectorId;

      CommentEditViewModel comment = new CommentEditViewModel();
      comment.Comment = _postCommentRepository.GetPostCommentById(commentId);
      comment.Files = _postCommentFileRepository.GetFilesByPostCommentId(commentId).OrderBy(a => a.Name).ToList();

      return View("Default", comment);
    }
  }
}