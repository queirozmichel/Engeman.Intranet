using Engeman.Intranet.Models;
using System.Collections.Generic;

namespace Engeman.Intranet.Repositories
{
  public interface IPostRepository
  {
    public List<PostDto> GetAllPosts();
    public Post GetPostById(int id);
    public void InsertQuestion(AskQuestionDto askQuestionDto);
    public void DeletePost(int postId);
  }
}