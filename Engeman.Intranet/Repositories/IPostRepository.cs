﻿using Engeman.Intranet.Models;
using Engeman.Intranet.Models.ViewModels;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<PostDto> GetPostsByRestriction(UserAccount user);
    public List<PostDto> GetPostsWithUnrevisedComments();
    public Post GetPostById(int id);
    public int GetPostsCountByUserId(int id);
    public List<int> GetRestrictedDepartmentsIdByPost(int id);
    public void AddQuestion(NewPostViewModel newPost);
    public void UpdateQuestion(EditedPostViewModel editedPost);
    public void UpdatePostFile(EditedPostWithFilesViewModel editedPostWithFiles);
    public void AddPostWithFile(NewPostWithFilesViewModel newPostWithFile);
    public void AddPostFile(int postId, List<NewPostFileViewModel> files);
    public void DeletePost(int postId);
    public bool UpdatePost(int id, Post post);
  }
}