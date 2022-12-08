using Engeman.Intranet.Models.ViewModels;
using Engeman.Intranet.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.Design;
using System.Linq;

namespace Engeman.Intranet.ViewComponents
{
    public class CommentEditFormViewComponent : ViewComponent
  {
    private readonly ICommentRepository _postCommentRepository;
    private readonly ICommentFileRepository _postCommentFileRepository;

    public CommentEditFormViewComponent(ICommentRepository postCommentRepository, ICommentFileRepository postCommentFileRepository)
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
      comment.Comment = _postCommentRepository.GetById(commentId);
      comment.Files = _postCommentFileRepository.GetByCommentId(commentId).OrderBy(a => a.Name).ToList();

      return View("Default", comment);
    }
  }
}