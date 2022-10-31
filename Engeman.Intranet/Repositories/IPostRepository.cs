using Engeman.Intranet.Models;
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
    public void AddQuestion(AskQuestionDto askQuestionDto);
    public void UpdateQuestion(int id, AskQuestionDto askQuestionDto);
    public void UpdatePostFile(int id, AskQuestionDto postInformation, List<PostFile> files);
    public void AddPostFile(AskQuestionDto askQuestionDto, List<PostFile> files);
    public void AddPostFile(int id, List<PostFile> files);
    public void DeletePost(int postId);
    public bool UpdatePost(int id, Post post);
  }
}